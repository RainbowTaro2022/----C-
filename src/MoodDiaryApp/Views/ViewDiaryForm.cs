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
    public partial class ViewDiaryForm : Form
    {
        private User currentUser;
        private List<MoodRecord> moodRecords;
        
        public ViewDiaryForm(User user)
        {
            currentUser = user;
            InitializeComponent();
            LoadMoodRecords();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // 设置窗体属性
            this.Text = "查看心情日记";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // 创建控件
            Label lblTitle = new Label();
            lblTitle.Text = "心情日记列表";
            lblTitle.Font = new Font("微软雅黑", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.Purple;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(400, 20);
            
            // 创建工具栏
            Button btnAdd = new Button();
            btnAdd.Text = "添加日记";
            btnAdd.Size = new Size(80, 30);
            btnAdd.Location = new Point(50, 70);
            btnAdd.Click += BtnAdd_Click;
            
            Button btnEdit = new Button();
            btnEdit.Text = "编辑日记";
            btnEdit.Size = new Size(80, 30);
            btnEdit.Location = new Point(150, 70);
            btnEdit.Click += BtnEdit_Click;
            
            Button btnDelete = new Button();
            btnDelete.Text = "删除日记";
            btnDelete.Size = new Size(80, 30);
            btnDelete.Location = new Point(250, 70);
            btnDelete.Click += BtnDelete_Click;
            
            Button btnRefresh = new Button();
            btnRefresh.Text = "刷新";
            btnRefresh.Size = new Size(80, 30);
            btnRefresh.Location = new Point(350, 70);
            btnRefresh.Click += BtnRefresh_Click;
            
            // 创建日期筛选控件
            Label lblDateRange = new Label();
            lblDateRange.Text = "日期范围:";
            lblDateRange.AutoSize = true;
            lblDateRange.Location = new Point(450, 75);
            
            DateTimePicker dtpStartDate = new DateTimePicker();
            dtpStartDate.Name = "dtpStartDate";
            dtpStartDate.Size = new Size(120, 25);
            dtpStartDate.Location = new Point(520, 72);
            dtpStartDate.Value = DateTime.Now.AddMonths(-1);
            
            Label lblTo = new Label();
            lblTo.Text = "至";
            lblTo.AutoSize = true;
            lblTo.Location = new Point(650, 75);
            
            DateTimePicker dtpEndDate = new DateTimePicker();
            dtpEndDate.Name = "dtpEndDate";
            dtpEndDate.Size = new Size(120, 25);
            dtpEndDate.Location = new Point(670, 72);
            dtpEndDate.Value = DateTime.Now;
            
            Button btnFilter = new Button();
            btnFilter.Text = "筛选";
            btnFilter.Size = new Size(60, 25);
            btnFilter.Location = new Point(800, 72);
            btnFilter.Click += BtnFilter_Click;
            
            // 创建搜索控件
            Label lblSearch = new Label();
            lblSearch.Text = "搜索:";
            lblSearch.AutoSize = true;
            lblSearch.Location = new Point(50, 105);
            
            TextBox txtSearch = new TextBox();
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(200, 25);
            txtSearch.Location = new Point(100, 102);
            txtSearch.PlaceholderText = "输入关键词搜索...";
            
            Button btnSearch = new Button();
            btnSearch.Text = "搜索";
            btnSearch.Size = new Size(60, 25);
            btnSearch.Location = new Point(310, 102);
            btnSearch.Click += BtnSearch_Click;
            
            // 创建心情日记列表
            DataGridView dgvMoodRecords = new DataGridView();
            dgvMoodRecords.Name = "dgvMoodRecords";
            dgvMoodRecords.Size = new Size(950, 400);
            dgvMoodRecords.Location = new Point(20, 120);
            dgvMoodRecords.AutoGenerateColumns = false;
            dgvMoodRecords.ReadOnly = true;
            dgvMoodRecords.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMoodRecords.MultiSelect = false;
            
            // 添加列
            dgvMoodRecords.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "RecordDate",
                HeaderText = "日期",
                DataPropertyName = "RecordDate",
                Width = 150
            });
            
            dgvMoodRecords.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "MoodText",
                HeaderText = "心情内容",
                DataPropertyName = "MoodText",
                Width = 400
            });
            
            dgvMoodRecords.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "MoodScore",
                HeaderText = "情绪评分",
                DataPropertyName = "MoodScore",
                Width = 80
            });
            
            dgvMoodRecords.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "PrivacyMode",
                HeaderText = "隐私模式",
                Width = 80,
                DataPropertyName = "PrivacyMode"
            });
            
            dgvMoodRecords.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "LikeCount",
                HeaderText = "点赞数",
                DataPropertyName = "LikeCount",
                Width = 80
            });
            
            dgvMoodRecords.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "CommentCount",
                HeaderText = "评论数",
                DataPropertyName = "CommentCount",
                Width = 80
            });
            
            // 设置隐私模式列的显示格式
            dgvMoodRecords.Columns["PrivacyMode"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            
            // 查看详情按钮
            Button btnViewDetails = new Button();
            btnViewDetails.Text = "查看详情";
            btnViewDetails.Size = new Size(80, 30);
            btnViewDetails.Location = new Point(50, 540);
            btnViewDetails.Click += BtnViewDetails_Click;
            
            // 点赞按钮
            Button btnLike = new Button();
            btnLike.Text = "点赞";
            btnLike.Size = new Size(80, 30);
            btnLike.Location = new Point(150, 540);
            btnLike.Click += BtnLike_Click;
            
            // 评论按钮
            Button btnComment = new Button();
            btnComment.Text = "评论";
            btnComment.Size = new Size(80, 30);
            btnComment.Location = new Point(250, 540);
            btnComment.Click += BtnComment_Click;
            
            // 添加控件到窗体
            this.Controls.Add(lblTitle);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(lblDateRange);
            this.Controls.Add(dtpStartDate);
            this.Controls.Add(lblTo);
            this.Controls.Add(dtpEndDate);
            this.Controls.Add(btnFilter);
            this.Controls.Add(lblSearch);
            this.Controls.Add(txtSearch);
            this.Controls.Add(btnSearch);
            this.Controls.Add(dgvMoodRecords);
            this.Controls.Add(btnViewDetails);
            this.Controls.Add(btnLike);
            this.Controls.Add(btnComment);
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadMoodRecords(DateTime? startDate = null, DateTime? endDate = null, string searchKeyword = null)
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
                
                // 应用搜索过滤
                if (!string.IsNullOrWhiteSpace(searchKeyword))
                {
                    moodRecords = moodRecords.Where(r => 
                        (!string.IsNullOrEmpty(r.MoodText) && r.MoodText.Contains(searchKeyword)) ||
                        r.RecordDate.ToString("yyyy-MM-dd").Contains(searchKeyword) ||
                        r.MoodScore.ToString().Contains(searchKeyword)
                    ).ToList();
                }
                
                DataGridView dgvMoodRecords = this.Controls.Find("dgvMoodRecords", true).FirstOrDefault() as DataGridView;
                if (dgvMoodRecords != null)
                {
                    // 创建绑定数据源，包含点赞数和评论数
                    var bindingSource = new BindingSource();
                    bindingSource.DataSource = moodRecords.Select(r => new {
                        RecordDate = r.RecordDate.ToString("yyyy-MM-dd"),
                        MoodText = r.MoodText,
                        MoodScore = r.MoodScore,
                        PrivacyMode = r.PrivacyMode == 1 ? "公开" : "私密",
                        LikeCount = LikeDAO.GetLikeCountByRecordId(r.RecordId),
                        CommentCount = CommentDAO.GetCommentCountByRecordId(r.RecordId)
                    }).ToList();
                    
                    dgvMoodRecords.DataSource = bindingSource;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载心情日记失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            using (var addForm = new AddDiaryForm(currentUser))
            {
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    LoadMoodRecords();
                }
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            DataGridView dgvMoodRecords = this.Controls.Find("dgvMoodRecords", true).FirstOrDefault() as DataGridView;
            if (dgvMoodRecords?.SelectedRows?.Count > 0)
            {
                var selectedRow = dgvMoodRecords.SelectedRows[0];
                var recordDate = DateTime.Parse(selectedRow.Cells["RecordDate"].Value.ToString());
                
                // 查找选中的记录
                var selectedRecord = moodRecords.FirstOrDefault(r => r.RecordDate.Date == recordDate.Date);
                if (selectedRecord != null)
                {
                    using (var editForm = new AddDiaryForm(currentUser, selectedRecord))
                    {
                        if (editForm.ShowDialog() == DialogResult.OK)
                        {
                            LoadMoodRecords();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("请先选择要编辑的日记", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            DataGridView dgvMoodRecords = this.Controls.Find("dgvMoodRecords", true).FirstOrDefault() as DataGridView;
            if (dgvMoodRecords?.SelectedRows?.Count > 0)
            {
                var result = MessageBox.Show("确定要删除选中的日记吗?", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    var selectedRow = dgvMoodRecords.SelectedRows[0];
                    var recordDate = DateTime.Parse(selectedRow.Cells["RecordDate"].Value.ToString());
                    
                    // 查找选中的记录
                    var selectedRecord = moodRecords.FirstOrDefault(r => r.RecordDate.Date == recordDate.Date);
                    if (selectedRecord != null)
                    {
                        try
                        {
                            if (MoodRecordDAO.DeleteMoodRecord(selectedRecord.RecordId))
                            {
                                MessageBox.Show("日记删除成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadMoodRecords();
                            }
                            else
                            {
                                MessageBox.Show("日记删除失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"删除失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("请先选择要删除的日记", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadMoodRecords();
        }

        private void BtnFilter_Click(object? sender, EventArgs e)
        {
            DateTimePicker dtpStartDate = this.Controls.Find("dtpStartDate", true).FirstOrDefault() as DateTimePicker;
            DateTimePicker dtpEndDate = this.Controls.Find("dtpEndDate", true).FirstOrDefault() as DateTimePicker;
            TextBox txtSearch = this.Controls.Find("txtSearch", true).FirstOrDefault() as TextBox;
            
            if (dtpStartDate != null && dtpEndDate != null)
            {
                if (dtpStartDate.Value <= dtpEndDate.Value)
                {
                    LoadMoodRecords(dtpStartDate.Value, dtpEndDate.Value, txtSearch?.Text);
                }
                else
                {
                    MessageBox.Show("开始日期不能晚于结束日期", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            TextBox txtSearch = this.Controls.Find("txtSearch", true).FirstOrDefault() as TextBox;
            DateTimePicker dtpStartDate = this.Controls.Find("dtpStartDate", true).FirstOrDefault() as DateTimePicker;
            DateTimePicker dtpEndDate = this.Controls.Find("dtpEndDate", true).FirstOrDefault() as DateTimePicker;
            
            // 获取日期范围
            DateTime? startDate = dtpStartDate?.Value;
            DateTime? endDate = dtpEndDate?.Value;
            
            // 如果设置了日期范围，验证日期
            if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
            {
                MessageBox.Show("开始日期不能晚于结束日期", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            LoadMoodRecords(startDate, endDate, txtSearch?.Text);
        }

        private void BtnViewDetails_Click(object? sender, EventArgs e)
        {
            DataGridView dgvMoodRecords = this.Controls.Find("dgvMoodRecords", true).FirstOrDefault() as DataGridView;
            if (dgvMoodRecords?.SelectedRows?.Count > 0)
            {
                var selectedRow = dgvMoodRecords.SelectedRows[0];
                var recordDate = DateTime.Parse(selectedRow.Cells["RecordDate"].Value.ToString());
                
                // 查找选中的记录
                var selectedRecord = moodRecords.FirstOrDefault(r => r.RecordDate.Date == recordDate.Date);
                if (selectedRecord != null)
                {
                    // 显示详细信息
                    StringBuilder details = new StringBuilder();
                    details.AppendLine($"日期: {selectedRecord.RecordDate:yyyy-MM-dd}");
                    details.AppendLine($"心情: {selectedRecord.MoodText}");
                    details.AppendLine($"情绪评分: {selectedRecord.MoodScore}");
                    details.AppendLine($"隐私模式: {(selectedRecord.PrivacyMode == 1 ? "公开" : "私密")}");
                    details.AppendLine($"位置: {selectedRecord.Location ?? "未指定"}");
                    details.AppendLine($"创建时间: {selectedRecord.CreatedAt:yyyy-MM-dd HH:mm:ss}");
                    details.AppendLine($"点赞数: {LikeDAO.GetLikeCountByRecordId(selectedRecord.RecordId)}");
                    details.AppendLine($"评论数: {CommentDAO.GetCommentCountByRecordId(selectedRecord.RecordId)}");
                    
                    MessageBox.Show(details.ToString(), "心情日记详情", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("请先选择一条日记记录", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnLike_Click(object? sender, EventArgs e)
        {
            DataGridView dgvMoodRecords = this.Controls.Find("dgvMoodRecords", true).FirstOrDefault() as DataGridView;
            if (dgvMoodRecords?.SelectedRows?.Count > 0)
            {
                var selectedRow = dgvMoodRecords.SelectedRows[0];
                var recordDate = DateTime.Parse(selectedRow.Cells["RecordDate"].Value.ToString());
                
                // 查找选中的记录
                var selectedRecord = moodRecords.FirstOrDefault(r => r.RecordDate.Date == recordDate.Date);
                if (selectedRecord != null)
                {
                    try
                    {
                        // 尝试添加点赞
                        if (LikeDAO.AddLike(selectedRecord.RecordId, currentUser.UserId))
                        {
                            MessageBox.Show("点赞成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadMoodRecords(); // 刷新显示
                        }
                        else
                        {
                            // 如果已经点赞，则取消点赞
                            if (LikeDAO.RemoveLike(selectedRecord.RecordId, currentUser.UserId))
                            {
                                MessageBox.Show("已取消点赞", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadMoodRecords(); // 刷新显示
                            }
                            else
                            {
                                MessageBox.Show("操作失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"点赞操作失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("请先选择一条日记记录", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnComment_Click(object? sender, EventArgs e)
        {
            DataGridView dgvMoodRecords = this.Controls.Find("dgvMoodRecords", true).FirstOrDefault() as DataGridView;
            if (dgvMoodRecords?.SelectedRows?.Count > 0)
            {
                var selectedRow = dgvMoodRecords.SelectedRows[0];
                var recordDate = DateTime.Parse(selectedRow.Cells["RecordDate"].Value.ToString());
                
                // 查找选中的记录
                var selectedRecord = moodRecords.FirstOrDefault(r => r.RecordDate.Date == recordDate.Date);
                if (selectedRecord != null)
                {
                    // 显示评论输入框
                    using (var commentForm = new CommentForm(currentUser, selectedRecord))
                    {
                        if (commentForm.ShowDialog() == DialogResult.OK)
                        {
                            LoadMoodRecords(); // 刷新显示
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("请先选择一条日记记录", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}