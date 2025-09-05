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
    public partial class MoodDistributionForm : Form
    {
        private User currentUser;
        private List<MoodRecord> moodRecords;
        
        public MoodDistributionForm(User user)
        {
            currentUser = user;
            InitializeComponent();
            LoadMoodRecords();
            GenerateMoodDistributionChart();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // 设置窗体属性
            this.Text = "情绪分布统计";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            
            // 创建控件
            Label lblTitle = new Label();
            lblTitle.Text = "情绪分布统计";
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
            Chart chartMoodDistribution = new Chart();
            chartMoodDistribution.Name = "chartMoodDistribution";
            chartMoodDistribution.Size = new Size(800, 400);
            chartMoodDistribution.Location = new Point(50, 120);
            
            ChartArea chartArea = new ChartArea("情绪分布");
            chartMoodDistribution.ChartAreas.Add(chartArea);
            
            // 添加控件到窗体
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblDateRange);
            this.Controls.Add(dtpStartDate);
            this.Controls.Add(lblTo);
            this.Controls.Add(dtpEndDate);
            this.Controls.Add(btnFilter);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(chartMoodDistribution);
            
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

        private void GenerateMoodDistributionChart()
        {
            Chart chartMoodDistribution = this.Controls.Find("chartMoodDistribution", true).FirstOrDefault() as Chart;
            if (chartMoodDistribution != null && moodRecords != null)
            {
                // 清除现有数据
                chartMoodDistribution.Series.Clear();
                
                // 统计情绪分布
                var moodDistribution = moodRecords
                    .GroupBy(r => r.MoodScore)
                    .Select(g => new { Score = g.Key, Count = g.Count() })
                    .OrderBy(x => x.Score)
                    .ToList();
                
                // 创建数据系列
                Series series = new Series("情绪分布");
                series.ChartType = SeriesChartType.Pie;
                
                // 定义颜色
                Color[] colors = { Color.DarkRed, Color.Red, Color.Orange, Color.Yellow, Color.LightGreen, Color.Green, Color.DarkGreen };
                
                // 添加数据点
                for (int i = 0; i < moodDistribution.Count; i++)
                {
                    var item = moodDistribution[i];
                    var point = new DataPoint();
                    point.SetValueXY(GetMoodLabel(item.Score), item.Count);
                    
                    // 设置颜色
                    if (i < colors.Length)
                        point.Color = colors[i];
                    
                    series.Points.Add(point);
                }
                
                // 添加到图表
                chartMoodDistribution.Series.Add(series);
                
                // 设置图表标题
                chartMoodDistribution.Titles.Clear();
                chartMoodDistribution.Titles.Add("情绪分布统计");
                
                // 设置图例
                chartMoodDistribution.Legends.Clear();
                var legend = new Legend();
                legend.Title = "情绪评分";
                chartMoodDistribution.Legends.Add(legend);
            }
        }

        private string GetMoodLabel(int score)
        {
            switch (score)
            {
                case -3: return "绝望(-3)";
                case -2: return "悲伤(-2)";
                case -1: return "沮丧(-1)";
                case 0: return "平静(0)";
                case 1: return "满足(1)";
                case 2: return "高兴(2)";
                case 3: return "兴奋(3)";
                default: return score.ToString();
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
                    GenerateMoodDistributionChart();
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
            GenerateMoodDistributionChart();
        }
    }
}