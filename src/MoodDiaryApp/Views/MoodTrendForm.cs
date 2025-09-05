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
using System.Windows.Forms.DataVisualization.Charting;

namespace MoodDiaryApp.Views
{
    public partial class MoodTrendForm : Form
    {
        private User currentUser;
        private List<MoodRecord> moodRecords;
        
        public MoodTrendForm(User user)
        {
            currentUser = user;
            InitializeComponent();
            LoadMoodRecords();
            GenerateMoodTrendChart();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // 设置窗体属性
            this.Text = "情绪趋势分析";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            
            // 创建控件
            Label lblTitle = new Label();
            lblTitle.Text = "情绪趋势分析";
            lblTitle.Font = new Font("微软雅黑", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.Purple;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(350, 20);
            
            // 创建日期筛选控件
            Label lblDateRange = new Label();
            lblDateRange.Text = "日期范围:";
            lblDateRange.AutoSize = true;
            lblDateRange.Location = new Point(50, 75);
            
            DateTimePicker dtpStartDate = new DateTimePicker();
            dtpStartDate.Name = "dtpStartDate";
            dtpStartDate.Size = new Size(120, 25);
            dtpStartDate.Location = new Point(120, 72);
            dtpStartDate.Value = DateTime.Now.AddMonths(-3);
            
            Label lblTo = new Label();
            lblTo.Text = "至";
            lblTo.AutoSize = true;
            lblTo.Location = new Point(250, 75);
            
            DateTimePicker dtpEndDate = new DateTimePicker();
            dtpEndDate.Name = "dtpEndDate";
            dtpEndDate.Size = new Size(120, 25);
            dtpEndDate.Location = new Point(270, 72);
            dtpEndDate.Value = DateTime.Now;
            
            Button btnFilter = new Button();
            btnFilter.Text = "筛选";
            btnFilter.Size = new Size(60, 25);
            btnFilter.Location = new Point(400, 72);
            btnFilter.Click += BtnFilter_Click;
            
            Button btnRefresh = new Button();
            btnRefresh.Text = "刷新";
            btnRefresh.Size = new Size(60, 25);
            btnRefresh.Location = new Point(470, 72);
            btnRefresh.Click += BtnRefresh_Click;
            
            // 创建图表控件
            Chart chartMoodTrend = new Chart();
            chartMoodTrend.Name = "chartMoodTrend";
            chartMoodTrend.Size = new Size(800, 400);
            chartMoodTrend.Location = new Point(50, 120);
            
            ChartArea chartArea = new ChartArea("情绪趋势");
            chartArea.AxisX.Title = "日期";
            chartArea.AxisY.Title = "情绪评分";
            chartArea.AxisY.Minimum = -3;
            chartArea.AxisY.Maximum = 3;
            chartMoodTrend.ChartAreas.Add(chartArea);
            
            // 添加控件到窗体
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblDateRange);
            this.Controls.Add(dtpStartDate);
            this.Controls.Add(lblTo);
            this.Controls.Add(dtpEndDate);
            this.Controls.Add(btnFilter);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(chartMoodTrend);
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadMoodRecords(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                if (startDate.HasValue && endDate.HasValue)
                {
                    moodRecords = MoodRecordDAO.GetMoodRecordsByDateRange(currentUser.UserId, startDate.Value, endDate.Value);
                }
                else
                {
                    moodRecords = MoodRecordDAO.GetMoodRecordsByUserId(currentUser.UserId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载心情日记失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateMoodTrendChart()
        {
            Chart chartMoodTrend = this.Controls.Find("chartMoodTrend", true).FirstOrDefault() as Chart;
            if (chartMoodTrend != null && moodRecords != null)
            {
                // 清除现有数据
                chartMoodTrend.Series.Clear();
                
                // 生成情绪趋势数据
                var trendData = MoodAnalysisService.GenerateMoodTrendData(moodRecords);
                
                // 创建数据系列
                Series series = new Series("情绪趋势");
                series.ChartType = SeriesChartType.Line;
                series.MarkerStyle = MarkerStyle.Circle;
                series.MarkerSize = 8;
                series.BorderWidth = 3;
                
                // 添加数据点
                foreach (var dataPoint in trendData)
                {
                    series.Points.AddXY(dataPoint.Key.ToString("MM-dd"), dataPoint.Value);
                }
                
                // 设置颜色
                series.Color = Color.Purple;
                
                // 添加到图表
                chartMoodTrend.Series.Add(series);
                
                // 设置图表标题
                chartMoodTrend.Titles.Clear();
                chartMoodTrend.Titles.Add("情绪变化趋势");
            }
        }

        private void BtnFilter_Click(object? sender, EventArgs e)
        {
            DateTimePicker dtpStartDate = this.Controls.Find("dtpStartDate", true).FirstOrDefault() as DateTimePicker;
            DateTimePicker dtpEndDate = this.Controls.Find("dtpEndDate", true).FirstOrDefault() as DateTimePicker;
            
            if (dtpStartDate != null && dtpEndDate != null)
            {
                if (dtpStartDate.Value <= dtpEndDate.Value)
                {
                    LoadMoodRecords(dtpStartDate.Value, dtpEndDate.Value);
                    GenerateMoodTrendChart();
                }
                else
                {
                    MessageBox.Show("开始日期不能晚于结束日期", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadMoodRecords();
            GenerateMoodTrendChart();
        }
    }
}