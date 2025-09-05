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

namespace MoodDiaryApp.Views
{
    public partial class CommentForm : Form
    {
        private User currentUser;
        private MoodRecord moodRecord;
        
        public CommentForm(User user, MoodRecord record)
        {
            currentUser = user;
            moodRecord = record;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // 设置窗体属性
            this.Text = "添加评论";
            this.Size = new Size(500, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            // 创建控件
            Label lblTitle = new Label();
            lblTitle.Text = "添加评论";
            lblTitle.Font = new Font("微软雅黑", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.Purple;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(200, 20);
            
            // 评论内容标签
            Label lblComment = new Label();
            lblComment.Text = "评论内容:";
            lblComment.AutoSize = true;
            lblComment.Location = new Point(30, 80);
            
            // 评论内容文本框
            TextBox txtComment = new TextBox();
            txtComment.Name = "txtComment";
            txtComment.Size = new Size(420, 100);
            txtComment.Location = new Point(30, 100);
            txtComment.Multiline = true;
            txtComment.ScrollBars = ScrollBars.Vertical;
            
            // 提交按钮
            Button btnSubmit = new Button();
            btnSubmit.Text = "提交";
            btnSubmit.Size = new Size(80, 30);
            btnSubmit.Location = new Point(150, 220);
            btnSubmit.Click += BtnSubmit_Click;
            
            // 取消按钮
            Button btnCancel = new Button();
            btnCancel.Text = "取消";
            btnCancel.Size = new Size(80, 30);
            btnCancel.Location = new Point(250, 220);
            btnCancel.Click += BtnCancel_Click;
            
            // 添加控件到窗体
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblComment);
            this.Controls.Add(txtComment);
            this.Controls.Add(btnSubmit);
            this.Controls.Add(btnCancel);
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void BtnSubmit_Click(object? sender, EventArgs e)
        {
            // 获取控件
            TextBox txtComment = this.Controls.Find("txtComment", true).FirstOrDefault() as TextBox;
            
            // 验证输入
            if (string.IsNullOrWhiteSpace(txtComment?.Text))
            {
                MessageBox.Show("评论内容不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // 创建评论对象
            var comment = new Comment
            {
                RecordId = moodRecord.RecordId,
                UserId = currentUser.UserId,
                CommentText = txtComment.Text.Trim()
            };
            
            // 保存到数据库
            try
            {
                int commentId = CommentDAO.AddComment(comment);
                if (commentId > 0)
                {
                    MessageBox.Show("评论添加成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("评论添加失败，请重试", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"评论添加失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}