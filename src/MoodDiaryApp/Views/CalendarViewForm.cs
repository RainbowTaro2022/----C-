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
    public partial class CalendarViewForm : Form
    {
        private User currentUser;
        private List<MoodRecord> moodRecords;
        private MonthCalendar monthCalendar;
        private TextBox txtMoodDetails;
        
        public CalendarViewForm(User user)
        {
            currentUser = user;
            InitializeComponent();
            LoadMoodRecords();
            HighlightMoodDates();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // 设置窗体属性
            this.Text = "日历视图";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // 创建控件
            Label lblTitle = new Label();
            lblTitle.Text = "心情日历";
            lblTitle.Font = new Font("微软雅黑", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.Purple;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(350, 20);
            
            // 创建MonthCalendar控件
            monthCalendar = new MonthCalendar();
            monthCalendar.Name = "monthCalendar";
            monthCalendar.Size = new Size(250, 200);
            monthCalendar.Location = new Point(50, 80);
            monthCalendar.MaxSelectionCount = 1;
            monthCalendar.DateSelected += MonthCalendar_DateSelected;
            
            // 创建心情详情文本框
            txtMoodDetails = new TextBox();
            txtMoodDetails.Name = "txtMoodDetails";
            txtMoodDetails.Size = new Size(500, 200);
            txtMoodDetails.Location = new Point(320, 80);
            txtMoodDetails.Multiline = true;
            txtMoodDetails.ScrollBars = ScrollBars.Vertical;
            txtMoodDetails.ReadOnly = true;
            
            // 创建操作按钮
            Button btnAddDiary = new Button();
            btnAddDiary.Text = "添加日记";
            btnAddDiary.Size = new Size(80, 30);
            btnAddDiary.Location = new Point(320, 300);
            btnAddDiary.Click += BtnAddDiary_Click;
            
            Button btnEditDiary = new Button();
            btnEditDiary.Text = "编辑日记";
            btnEditDiary.Size = new Size(80, 30);
            btnEditDiary.Location = new Point(420, 300);
            btnEditDiary.Click += BtnEditDiary_Click;
            
            Button btnRefresh = new Button();
            btnRefresh.Text = "刷新";
            btnRefresh.Size = new Size(80, 30);
            btnRefresh.Location = new Point(520, 300);
            btnRefresh.Click += BtnRefresh_Click;
            
            // 添加控件到窗体
            this.Controls.Add(lblTitle);
            this.Controls.Add(monthCalendar);
            this.Controls.Add(txtMoodDetails);
            this.Controls.Add(btnAddDiary);
            this.Controls.Add(btnEditDiary);
            this.Controls.Add(btnRefresh);
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadMoodRecords()
        {
            try
            {
                moodRecords = MoodRecordDAO.GetMoodRecordsByUserId(currentUser.UserId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载心情日记失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HighlightMoodDates()
        {
            if (moodRecords != null && moodRecords.Any())
            {
                // 获取有心情记录的日期
                var moodDates = moodRecords.Select(r => r.RecordDate.Date).Distinct().ToArray();
                monthCalendar.BoldedDates = moodDates;
            }
        }

        private void MonthCalendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            DateTime selectedDate = e.Start.Date;
            
            // 查找选定日期的心情记录
            var recordsForDate = moodRecords.Where(r => r.RecordDate.Date == selectedDate).ToList();
            
            if (recordsForDate.Any())
            {
                // 显示心情详情
                var record = recordsForDate.First();
                txtMoodDetails.Text = $"日期: {record.RecordDate:yyyy-MM-dd}\r\n" +
                                     $"心情: {record.MoodText}\r\n" +
                                     $"情绪评分: {record.MoodScore}\r\n" +
                                     $"隐私模式: {(record.PrivacyMode == 1 ? "公开" : "私密")}\r\n" +
                                     $"位置: {record.Location ?? "未指定"}\r\n" +
                                     $"创建时间: {record.CreatedAt:yyyy-MM-dd HH:mm:ss}";
            }
            else
            {
                txtMoodDetails.Text = $"日期: {selectedDate:yyyy-MM-dd}\r\n" +
                                     "该日期没有心情记录";
            }
        }

        private void BtnAddDiary_Click(object? sender, EventArgs e)
        {
            DateTime selectedDate = monthCalendar.SelectionStart.Date;
            
            // 检查是否已有该日期的记录
            var existingRecord = moodRecords.FirstOrDefault(r => r.RecordDate.Date == selectedDate);
            
            if (existingRecord != null)
            {
                MessageBox.Show("该日期已有心情记录，如需修改请选择编辑功能", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            // 创建新记录
            var newRecord = new MoodRecord 
            { 
                UserId = currentUser.UserId, 
                RecordDate = selectedDate,
                CreatedAt = DateTime.Now
            };
            
            using (var addForm = new AddDiaryForm(currentUser, newRecord))
            {
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    LoadMoodRecords();
                    HighlightMoodDates();
                    txtMoodDetails.Text = "";
                }
            }
        }

        private void BtnEditDiary_Click(object? sender, EventArgs e)
        {
            DateTime selectedDate = monthCalendar.SelectionStart.Date;
            
            // 查找选定日期的心情记录
            var existingRecord = moodRecords.FirstOrDefault(r => r.RecordDate.Date == selectedDate);
            
            if (existingRecord == null)
            {
                MessageBox.Show("该日期没有心情记录，如需添加请选择添加功能", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            using (var editForm = new AddDiaryForm(currentUser, existingRecord))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadMoodRecords();
                    HighlightMoodDates();
                    txtMoodDetails.Text = "";
                }
            }
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadMoodRecords();
            HighlightMoodDates();
            txtMoodDetails.Text = "";
        }
    }
}