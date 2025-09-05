using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoodDiaryApp.Models;
using MoodDiaryApp.Data;
using System.IO;

namespace MoodDiaryApp.Views
{
    public partial class EmojiManagerForm : Form
    {
        private User currentUser;
        private List<Emoji> userEmojis;
        private FlowLayoutPanel flowLayoutPanel;
        private string emojiDirectory;
        
        public EmojiManagerForm(User user)
        {
            currentUser = user;
            // 创建表情包存储目录
            emojiDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MoodDiary", "Emojis");
            if (!Directory.Exists(emojiDirectory))
            {
                Directory.CreateDirectory(emojiDirectory);
            }
            
            InitializeComponent();
            LoadUserEmojis();
            DisplayEmojis();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // 设置窗体属性
            this.Text = "表情包管理";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            // 创建标题标签
            Label lblTitle = new Label();
            lblTitle.Text = "我的表情包";
            lblTitle.Font = new Font("微软雅黑", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.Purple;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(320, 20);
            
            // 创建上传按钮
            Button btnUpload = new Button();
            btnUpload.Text = "上传表情包";
            btnUpload.Size = new Size(100, 30);
            btnUpload.Location = new Point(50, 70);
            btnUpload.Click += BtnUpload_Click;
            
            // 创建刷新按钮
            Button btnRefresh = new Button();
            btnRefresh.Text = "刷新";
            btnRefresh.Size = new Size(80, 30);
            btnRefresh.Location = new Point(170, 70);
            btnRefresh.Click += BtnRefresh_Click;
            
            // 创建说明标签
            Label lblInstruction = new Label();
            lblInstruction.Text = "点击表情包可删除";
            lblInstruction.AutoSize = true;
            lblInstruction.Location = new Point(50, 110);
            lblInstruction.ForeColor = Color.Gray;
            
            // 创建FlowLayoutPanel用于显示表情包
            flowLayoutPanel = new FlowLayoutPanel();
            flowLayoutPanel.Name = "flowLayoutPanel";
            flowLayoutPanel.Size = new Size(700, 400);
            flowLayoutPanel.Location = new Point(50, 140);
            flowLayoutPanel.AutoScroll = true;
            flowLayoutPanel.BorderStyle = BorderStyle.FixedSingle;
            
            // 添加控件到窗体
            this.Controls.Add(lblTitle);
            this.Controls.Add(btnUpload);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(lblInstruction);
            this.Controls.Add(flowLayoutPanel);
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadUserEmojis()
        {
            try
            {
                userEmojis = EmojiDAO.GetUserEmojis(currentUser.UserId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载表情包失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayEmojis()
        {
            flowLayoutPanel.Controls.Clear();
            
            if (userEmojis == null || userEmojis.Count == 0)
            {
                Label lblNoEmojis = new Label();
                lblNoEmojis.Text = "暂无自定义表情包";
                lblNoEmojis.AutoSize = true;
                lblNoEmojis.ForeColor = Color.Gray;
                flowLayoutPanel.Controls.Add(lblNoEmojis);
                return;
            }
            
            foreach (var emoji in userEmojis)
            {
                if (!string.IsNullOrEmpty(emoji.EmojiPath) && File.Exists(emoji.EmojiPath))
                {
                    try
                    {
                        PictureBox pictureBox = new PictureBox();
                        pictureBox.Size = new Size(80, 80);
                        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                        pictureBox.Image = Image.FromFile(emoji.EmojiPath);
                        pictureBox.Tag = emoji;
                        pictureBox.Click += PictureBox_Click;
                        pictureBox.BorderStyle = BorderStyle.FixedSingle;
                        pictureBox.Margin = new Padding(5);
                        pictureBox.Cursor = Cursors.Hand;
                        
                        // 添加工具提示
                        ToolTip toolTip = new ToolTip();
                        toolTip.SetToolTip(pictureBox, $"点击删除: {emoji.EmojiName}");
                        
                        flowLayoutPanel.Controls.Add(pictureBox);
                    }
                    catch (Exception ex)
                    {
                        // 如果图片加载失败，显示错误信息
                        Label lblError = new Label();
                        lblError.Text = $"❌ {emoji.EmojiName}";
                        lblError.AutoSize = true;
                        lblError.Tag = emoji;
                        lblError.Click += Label_Click;
                        lblError.Cursor = Cursors.Hand;
                        lblError.ForeColor = Color.Red;
                        flowLayoutPanel.Controls.Add(lblError);
                    }
                }
                else
                {
                    // 如果图片文件不存在，显示名称
                    Label lblMissing = new Label();
                    lblMissing.Text = $"❓ {emoji.EmojiName}";
                    lblMissing.AutoSize = true;
                    lblMissing.Tag = emoji;
                    lblMissing.Click += Label_Click;
                    lblMissing.Cursor = Cursors.Hand;
                    lblMissing.ForeColor = Color.Orange;
                    flowLayoutPanel.Controls.Add(lblMissing);
                }
            }
        }

        private void BtnUpload_Click(object? sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择表情包图片";
            openFileDialog.Filter = "图片文件|*.jpg;*.jpeg;*.png;*.gif;*.bmp|所有文件|*.*";
            openFileDialog.Multiselect = true;
            
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string filePath in openFileDialog.FileNames)
                {
                    try
                    {
                        // 复制文件到表情包目录
                        string fileName = Path.GetFileNameWithoutExtension(filePath);
                        string extension = Path.GetExtension(filePath);
                        string newFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
                        string newFilePath = Path.Combine(emojiDirectory, newFileName);
                        
                        File.Copy(filePath, newFilePath, true);
                        
                        // 创建Emoji对象
                        Emoji emoji = new Emoji
                        {
                            UserId = currentUser.UserId,
                            EmojiName = fileName,
                            EmojiPath = newFilePath
                        };
                        
                        // 保存到数据库
                        int emojiId = EmojiDAO.AddEmoji(emoji);
                        if (emojiId > 0)
                        {
                            emoji.EmojiId = emojiId;
                            userEmojis.Add(emoji);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"上传表情包失败 {Path.GetFileName(filePath)}: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                
                // 刷新显示
                LoadUserEmojis();
                DisplayEmojis();
                MessageBox.Show("表情包上传完成", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadUserEmojis();
            DisplayEmojis();
        }

        private void PictureBox_Click(object? sender, EventArgs e)
        {
            if (sender is PictureBox pictureBox && pictureBox.Tag is Emoji emoji)
            {
                DeleteEmoji(emoji);
            }
        }

        private void Label_Click(object? sender, EventArgs e)
        {
            if (sender is Label label && label.Tag is Emoji emoji)
            {
                DeleteEmoji(emoji);
            }
        }

        private void DeleteEmoji(Emoji emoji)
        {
            DialogResult result = MessageBox.Show($"确定要删除表情包 '{emoji.EmojiName}' 吗?", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    // 从数据库删除
                    if (EmojiDAO.DeleteEmoji(emoji.EmojiId))
                    {
                        // 从文件系统删除
                        if (!string.IsNullOrEmpty(emoji.EmojiPath) && File.Exists(emoji.EmojiPath))
                        {
                            try
                            {
                                File.Delete(emoji.EmojiPath);
                            }
                            catch
                            {
                                // 忽略文件删除错误
                            }
                        }
                        
                        // 从列表中移除
                        userEmojis.Remove(emoji);
                        
                        // 刷新显示
                        DisplayEmojis();
                        
                        MessageBox.Show("表情包删除成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("删除表情包失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"删除表情包失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
