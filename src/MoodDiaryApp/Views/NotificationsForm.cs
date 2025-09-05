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
    public partial class NotificationsForm : Form
    {
        private User currentUser;
        private List<NotificationItem> notifications;
        
        public NotificationsForm(User user)
        {
            currentUser = user;
            InitializeComponent();
            LoadNotifications();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // 设置窗体属性
            this.Text = "消息通知";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // 创建控件
            Label lblTitle = new Label();
            lblTitle.Text = "消息通知";
            lblTitle.Font = new Font("微软雅黑", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.Purple;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(350, 20);
            
            // 创建通知列表
            DataGridView dgvNotifications = new DataGridView();
            dgvNotifications.Name = "dgvNotifications";
            dgvNotifications.Size = new Size(750, 450);
            dgvNotifications.Location = new Point(20, 80);
            dgvNotifications.AutoGenerateColumns = false;
            dgvNotifications.ReadOnly = true;
            dgvNotifications.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvNotifications.MultiSelect = false;
            
            // 添加列
            dgvNotifications.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Type",
                HeaderText = "类型",
                DataPropertyName = "Type",
                Width = 100
            });
            
            dgvNotifications.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Content",
                HeaderText = "内容",
                DataPropertyName = "Content",
                Width = 400
            });
            
            dgvNotifications.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Sender",
                HeaderText = "发送者",
                DataPropertyName = "Sender",
                Width = 150
            });
            
            dgvNotifications.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Time",
                HeaderText = "时间",
                DataPropertyName = "Time",
                Width = 150
            });
            
            // 标记为已读按钮
            Button btnMarkAsRead = new Button();
            btnMarkAsRead.Text = "标记为已读";
            btnMarkAsRead.Size = new Size(100, 30);
            btnMarkAsRead.Location = new Point(550, 540);
            btnMarkAsRead.Click += BtnMarkAsRead_Click;
            
            // 刷新按钮
            Button btnRefresh = new Button();
            btnRefresh.Text = "刷新";
            btnRefresh.Size = new Size(80, 30);
            btnRefresh.Location = new Point(680, 540);
            btnRefresh.Click += BtnRefresh_Click;
            
            // 添加控件到窗体
            this.Controls.Add(lblTitle);
            this.Controls.Add(dgvNotifications);
            this.Controls.Add(btnMarkAsRead);
            this.Controls.Add(btnRefresh);
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadNotifications()
        {
            try
            {
                notifications = new List<NotificationItem>();
                
                // 获取好友请求
                var pendingFriends = FriendDAO.GetPendingFriendsByUserId(currentUser.UserId);
                foreach (var friend in pendingFriends)
                {
                    notifications.Add(new NotificationItem
                    {
                        Type = "好友请求",
                        Content = $"{friend.Username} 请求添加您为好友",
                        Sender = friend.Username,
                        Time = friend.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                        IsRead = false
                    });
                }
                
                // 获取点赞通知（这里简化处理，实际应该从好友的心情记录中获取）
                var userLikes = LikeDAO.GetLikesByUserId(currentUser.UserId);
                foreach (var like in userLikes.Take(10)) // 限制数量
                {
                    // 获取点赞用户信息
                    var likeUser = UserDAO.GetUserById(like.UserId);
                    if (likeUser != null && likeUser.UserId != currentUser.UserId)
                    {
                        notifications.Add(new NotificationItem
                        {
                            Type = "点赞",
                            Content = $"{likeUser.Username} 点赞了您的心情记录",
                            Sender = likeUser.Username,
                            Time = like.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                            IsRead = false
                        });
                    }
                }
                
                // 获取评论通知（这里简化处理）
                var userComments = CommentDAO.GetCommentsByUserId(currentUser.UserId);
                foreach (var comment in userComments.Take(10)) // 限制数量
                {
                    // 获取评论用户信息
                    var commentUser = UserDAO.GetUserById(comment.UserId);
                    if (commentUser != null && commentUser.UserId != currentUser.UserId)
                    {
                        notifications.Add(new NotificationItem
                        {
                            Type = "评论",
                            Content = $"{commentUser.Username} 评论了您的心情记录: {comment.CommentText}",
                            Sender = commentUser.Username,
                            Time = comment.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                            IsRead = false
                        });
                    }
                }
                
                // 按时间倒序排列
                notifications = notifications.OrderByDescending(n => n.Time).ToList();
                
                // 更新通知列表
                DataGridView dgvNotifications = this.Controls.Find("dgvNotifications", true).FirstOrDefault() as DataGridView;
                if (dgvNotifications != null)
                {
                    var bindingSource = new BindingSource();
                    bindingSource.DataSource = notifications;
                    dgvNotifications.DataSource = bindingSource;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载通知失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnMarkAsRead_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("标记为已读功能待实现", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadNotifications();
        }
    }
    
    // 通知项数据模型
    public class NotificationItem
    {
        public string Type { get; set; }
        public string Content { get; set; }
        public string Sender { get; set; }
        public string Time { get; set; }
        public bool IsRead { get; set; }
    }
}