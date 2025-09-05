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
    public partial class MoodTreeForm : Form
    {
        private User currentUser;
        private List<MoodRecord> moodRecords;
        private TreeView treeView;
        private RichTextBox txtDetails;
        
        public MoodTreeForm(User user)
        {
            currentUser = user;
            InitializeComponent();
            LoadMoodRecords();
            BuildTree();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // 设置窗体属性
            this.Text = "心情日志树形视图";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // 创建标题标签
            Label lblTitle = new Label();
            lblTitle.Text = "心情日志树形视图";
            lblTitle.Font = new Font("微软雅黑", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.Purple;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(400, 20);
            
            // 创建TreeView控件
            treeView = new TreeView();
            treeView.Name = "treeView";
            treeView.Size = new Size(400, 500);
            treeView.Location = new Point(20, 80);
            treeView.AfterSelect += TreeView_AfterSelect;
            
            // 创建详细信息文本框
            txtDetails = new RichTextBox();
            txtDetails.Name = "txtDetails";
            txtDetails.Size = new Size(500, 500);
            txtDetails.Location = new Point(450, 80);
            txtDetails.ReadOnly = true;
            
            // 创建刷新按钮
            Button btnRefresh = new Button();
            btnRefresh.Text = "刷新";
            btnRefresh.Size = new Size(80, 30);
            btnRefresh.Location = new Point(20, 600);
            btnRefresh.Click += BtnRefresh_Click;
            
            // 添加控件到窗体
            this.Controls.Add(lblTitle);
            this.Controls.Add(treeView);
            this.Controls.Add(txtDetails);
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

        private void BuildTree()
        {
            treeView.Nodes.Clear();
            
            if (moodRecords == null || moodRecords.Count == 0)
            {
                treeView.Nodes.Add("暂无心情记录");
                return;
            }
            
            // 按年份分组
            var recordsByYear = moodRecords
                .GroupBy(r => r.RecordDate.Year)
                .OrderByDescending(g => g.Key);
            
            foreach (var yearGroup in recordsByYear)
            {
                TreeNode yearNode = new TreeNode($" {yearGroup.Key}年");
                yearNode.Tag = yearGroup.Key;
                
                // 按月份分组
                var recordsByMonth = yearGroup
                    .GroupBy(r => r.RecordDate.Month)
                    .OrderByDescending(g => g.Key);
                
                foreach (var monthGroup in recordsByMonth)
                {
                    TreeNode monthNode = new TreeNode($"   {monthGroup.Key}月");
                    monthNode.Tag = new { Year = yearGroup.Key, Month = monthGroup.Key };
                    
                    // 添加每一天的记录
                    var recordsByDay = monthGroup
                        .OrderByDescending(r => r.RecordDate);
                    
                    foreach (var record in recordsByDay)
                    {
                        string nodeText = $"     {record.RecordDate:MM-dd} - {GetMoodTextPreview(record.MoodText)}";
                        TreeNode recordNode = new TreeNode(nodeText);
                        recordNode.Tag = record;
                        monthNode.Nodes.Add(recordNode);
                    }
                    
                    yearNode.Nodes.Add(monthNode);
                }
                
                treeView.Nodes.Add(yearNode);
            }
            
            // 展开所有节点
            treeView.ExpandAll();
        }
        
        private string GetMoodTextPreview(string moodText)
        {
            if (string.IsNullOrWhiteSpace(moodText))
                return "无内容";
                
            // 截取前20个字符作为预览
            string preview = moodText.Length > 20 ? moodText.Substring(0, 20) + "..." : moodText;
            return preview;
        }

        private void TreeView_AfterSelect(object? sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is MoodRecord record)
            {
                DisplayRecordDetails(record);
            }
            else
            {
                txtDetails.Clear();
            }
        }
        
        private void DisplayRecordDetails(MoodRecord record)
        {
            StringBuilder details = new StringBuilder();
            
            details.AppendLine("心情日记详情");
            details.AppendLine("==================");
            details.AppendLine($"日期: {record.RecordDate:yyyy-MM-dd HH:mm}");
            details.AppendLine($"情绪评分: {record.MoodScore}/10");
            details.AppendLine($"隐私模式: {(record.PrivacyMode == 1 ? "公开" : "私密")}");
            details.AppendLine($"位置: {record.Location ?? "未指定"}");
            details.AppendLine("");
            details.AppendLine("心情内容:");
            details.AppendLine(record.MoodText ?? "无内容");
            details.AppendLine("");
            details.AppendLine($"创建时间: {record.CreatedAt:yyyy-MM-dd HH:mm:ss}");
            details.AppendLine($"点赞数: {LikeDAO.GetLikeCountByRecordId(record.RecordId)}");
            details.AppendLine($"评论数: {CommentDAO.GetCommentCountByRecordId(record.RecordId)}");
            
            txtDetails.Text = details.ToString();
            
            // 设置情绪评分的颜色
            txtDetails.SelectAll();
            txtDetails.SelectionColor = Color.Black;
            
            // 找到情绪评分行并设置颜色
            string moodScoreLine = $"情绪评分: {record.MoodScore}/10";
            int scoreIndex = txtDetails.Text.IndexOf(moodScoreLine);
            if (scoreIndex >= 0)
            {
                txtDetails.Select(scoreIndex, moodScoreLine.Length);
                txtDetails.SelectionColor = GetMoodScoreColor(record.MoodScore);
            }
        }
        
        private Color GetMoodScoreColor(int moodScore)
        {
            // 根据情绪评分返回不同的颜色
            if (moodScore >= 8) return Color.Green;      // 高情绪
            if (moodScore >= 6) return Color.Blue;       // 中高情绪
            if (moodScore >= 4) return Color.Orange;     // 中等情绪
            if (moodScore >= 2) return Color.OrangeRed;  // 中低情绪
            return Color.Red;                            // 低情绪
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadMoodRecords();
            BuildTree();
        }
    }
}