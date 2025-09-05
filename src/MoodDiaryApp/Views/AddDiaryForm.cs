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
using MoodDiaryApp.Services;

namespace MoodDiaryApp.Views
{
    public partial class AddDiaryForm : Form
    {
        private User currentUser;
        private MoodRecord currentRecord;
        
        public AddDiaryForm(User user)
        {
            currentUser = user;
            currentRecord = new MoodRecord { UserId = user.UserId, RecordDate = DateTime.Now };
            InitializeComponent();
        }

        public AddDiaryForm(User user, MoodRecord record)
        {
            currentUser = user;
            currentRecord = record;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // 设置窗体属性
            this.Text = "添加心情日记";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            // 创建控件
            Label lblTitle = new Label();
            lblTitle.Text = "心情日记";
            lblTitle.Font = new Font("微软雅黑", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.Purple;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(250, 20);
            
            Label lblDate = new Label();
            lblDate.Text = "日期:";
            lblDate.AutoSize = true;
            lblDate.Location = new Point(50, 70);
            
            DateTimePicker dtpDate = new DateTimePicker();
            dtpDate.Name = "dtpDate";
            dtpDate.Size = new Size(200, 25);
            dtpDate.Location = new Point(120, 68);
            dtpDate.Value = currentRecord.RecordDate;
            
            Label lblMood = new Label();
            lblMood.Text = "心情:";
            lblMood.AutoSize = true;
            lblMood.Location = new Point(50, 110);
            
            TextBox txtMood = new TextBox();
            txtMood.Name = "txtMood";
            txtMood.Size = new Size(400, 100);
            txtMood.Location = new Point(120, 110);
            txtMood.Multiline = true;
            txtMood.ScrollBars = ScrollBars.Vertical;
            txtMood.Text = currentRecord.MoodText ?? "";
            
            Label lblTags = new Label();
            lblTags.Text = "情绪标签:";
            lblTags.AutoSize = true;
            lblTags.Location = new Point(50, 230);
            
            ComboBox cmbTags = new ComboBox();
            cmbTags.Name = "cmbTags";
            cmbTags.Size = new Size(200, 25);
            cmbTags.Location = new Point(120, 228);
            cmbTags.DropDownStyle = ComboBoxStyle.DropDownList;
            // 添加预定义的情绪标签
            cmbTags.Items.AddRange(new string[] { "开心", "悲伤", "愤怒", "焦虑", "平静", "兴奋", "沮丧", "满足" });
            
            Button btnSuggestTags = new Button();
            btnSuggestTags.Text = "智能推荐";
            btnSuggestTags.Size = new Size(80, 25);
            btnSuggestTags.Location = new Point(350, 228);
            btnSuggestTags.Click += BtnSuggestTags_Click;
            
            Label lblPrivacy = new Label();
            lblPrivacy.Text = "隐私设置:";
            lblPrivacy.AutoSize = true;
            lblPrivacy.Location = new Point(50, 270);
            
            RadioButton rbPublic = new RadioButton();
            rbPublic.Text = "公开";
            rbPublic.AutoSize = true;
            rbPublic.Location = new Point(120, 270);
            rbPublic.Checked = currentRecord.PrivacyMode == 1;
            
            RadioButton rbPrivate = new RadioButton();
            rbPrivate.Text = "私密";
            rbPrivate.AutoSize = true;
            rbPrivate.Location = new Point(200, 270);
            rbPrivate.Checked = currentRecord.PrivacyMode == 0;
            
            Label lblLocation = new Label();
            lblLocation.Text = "位置:";
            lblLocation.AutoSize = true;
            lblLocation.Location = new Point(50, 310);
            
            TextBox txtLocation = new TextBox();
            txtLocation.Name = "txtLocation";
            txtLocation.Size = new Size(200, 25);
            txtLocation.Location = new Point(120, 308);
            txtLocation.Text = currentRecord.Location ?? "";
            
            Button btnAdd = new Button();
            btnAdd.Text = currentRecord.RecordId > 0 ? "更新" : "添加";
            btnAdd.Size = new Size(80, 30);
            btnAdd.Location = new Point(150, 360);
            btnAdd.Click += BtnAdd_Click;
            
            Button btnCancel = new Button();
            btnCancel.Text = "取消";
            btnCancel.Size = new Size(80, 30);
            btnCancel.Location = new Point(250, 360);
            btnCancel.Click += BtnCancel_Click;
            
            // 添加控件到窗体
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblDate);
            this.Controls.Add(dtpDate);
            this.Controls.Add(lblMood);
            this.Controls.Add(txtMood);
            this.Controls.Add(lblTags);
            this.Controls.Add(cmbTags);
            this.Controls.Add(btnSuggestTags);
            this.Controls.Add(lblPrivacy);
            this.Controls.Add(rbPublic);
            this.Controls.Add(rbPrivate);
            this.Controls.Add(lblLocation);
            this.Controls.Add(txtLocation);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnCancel);
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void BtnSuggestTags_Click(object? sender, EventArgs e)
        {
            TextBox txtMood = this.Controls.Find("txtMood", true).FirstOrDefault() as TextBox;
            ComboBox cmbTags = this.Controls.Find("cmbTags", true).FirstOrDefault() as ComboBox;
            
            if (!string.IsNullOrWhiteSpace(txtMood?.Text))
            {
                var suggestedTags = MoodAnalysisService.GetSuggestedTags(txtMood.Text);
                if (suggestedTags.Any())
                {
                    MessageBox.Show($"推荐标签: {string.Join(", ", suggestedTags)}", "智能推荐", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // 选中第一个推荐标签
                    if (cmbTags.Items.Contains(suggestedTags[0]))
                    {
                        cmbTags.SelectedItem = suggestedTags[0];
                    }
                }
                else
                {
                    MessageBox.Show("未找到匹配的情绪标签", "智能推荐", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("请先输入心情内容", "提示", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            DateTimePicker dtpDate = this.Controls.Find("dtpDate", true).FirstOrDefault() as DateTimePicker;
            TextBox txtMood = this.Controls.Find("txtMood", true).FirstOrDefault() as TextBox;
            ComboBox cmbTags = this.Controls.Find("cmbTags", true).FirstOrDefault() as ComboBox;
            RadioButton rbPublic = this.Controls.Find("rbPublic", true).FirstOrDefault() as RadioButton;
            RadioButton rbPrivate = this.Controls.Find("rbPrivate", true).FirstOrDefault() as RadioButton;
            TextBox txtLocation = this.Controls.Find("txtLocation", true).FirstOrDefault() as TextBox;
            
            // 验证输入
            if (string.IsNullOrWhiteSpace(txtMood?.Text))
            {
                MessageBox.Show("请输入心情内容", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // 更新当前记录
            currentRecord.RecordDate = dtpDate.Value;
            currentRecord.MoodText = txtMood.Text;
            currentRecord.PrivacyMode = rbPublic.Checked ? 1 : 0;
            currentRecord.Location = txtLocation?.Text;
            
            // 进行情感分析
            currentRecord.MoodScore = MoodAnalysisService.AnalyzeMoodScore(txtMood.Text);
            
            try
            {
                if (currentRecord.RecordId > 0)
                {
                    // 更新记录
                    if (MoodRecordDAO.UpdateMoodRecord(currentRecord))
                    {
                        MessageBox.Show("心情日记更新成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("心情日记更新失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // 添加新记录
                    int recordId = MoodRecordDAO.AddMoodRecord(currentRecord);
                    if (recordId > 0)
                    {
                        MessageBox.Show("心情日记添加成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("心情日记添加失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"操作失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}