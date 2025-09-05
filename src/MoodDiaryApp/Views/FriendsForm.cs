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
    public partial class FriendsForm : Form
    {
        private User currentUser;
        private List<User> friends;
        private List<User> pendingFriends;
        
        public FriendsForm(User user)
        {
            currentUser = user;
            InitializeComponent();
            LoadFriends();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // 设置窗体属性
            this.Text = "好友列表";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // 创建控件
            Label lblTitle = new Label();
            lblTitle.Text = "好友管理";
            lblTitle.Font = new Font("微软雅黑", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.Purple;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(350, 20);
            
            // 创建标签页控件
            TabControl tabControl = new TabControl();
            tabControl.Name = "tabControl";
            tabControl.Size = new Size(750, 450);
            tabControl.Location = new Point(20, 80);
            
            // 好友列表标签页
            TabPage tabPageFriends = new TabPage("好友列表");
            tabPageFriends.Name = "tabPageFriends";
            
            // 好友列表
            DataGridView dgvFriends = new DataGridView();
            dgvFriends.Name = "dgvFriends";
            dgvFriends.Size = new Size(730, 350);
            dgvFriends.Location = new Point(10, 10);
            dgvFriends.AutoGenerateColumns = false;
            dgvFriends.ReadOnly = true;
            dgvFriends.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFriends.MultiSelect = false;
            
            // 添加列
            dgvFriends.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Username",
                HeaderText = "用户名",
                DataPropertyName = "Username",
                Width = 200
            });
            
            dgvFriends.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Email",
                HeaderText = "邮箱",
                DataPropertyName = "Email",
                Width = 250
            });
            
            dgvFriends.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "LastLogin",
                HeaderText = "最后登录",
                DataPropertyName = "LastLogin",
                Width = 150
            });
            
            // 删除好友按钮
            Button btnDeleteFriend = new Button();
            btnDeleteFriend.Text = "删除好友";
            btnDeleteFriend.Size = new Size(80, 30);
            btnDeleteFriend.Location = new Point(650, 370);
            btnDeleteFriend.Click += BtnDeleteFriend_Click;
            
            tabPageFriends.Controls.Add(dgvFriends);
            tabPageFriends.Controls.Add(btnDeleteFriend);
            
            // 好友请求标签页
            TabPage tabPageRequests = new TabPage("好友请求");
            tabPageRequests.Name = "tabPageRequests";
            
            // 好友请求列表
            DataGridView dgvRequests = new DataGridView();
            dgvRequests.Name = "dgvRequests";
            dgvRequests.Size = new Size(730, 350);
            dgvRequests.Location = new Point(10, 10);
            dgvRequests.AutoGenerateColumns = false;
            dgvRequests.ReadOnly = true;
            dgvRequests.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRequests.MultiSelect = false;
            
            // 添加列
            dgvRequests.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "RequestUsername",
                HeaderText = "用户名",
                DataPropertyName = "Username",
                Width = 200
            });
            
            dgvRequests.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "RequestEmail",
                HeaderText = "邮箱",
                DataPropertyName = "Email",
                Width = 250
            });
            
            dgvRequests.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "RequestDate",
                HeaderText = "请求时间",
                DataPropertyName = "CreatedAt",
                Width = 150
            });
            
            // 接受请求按钮
            Button btnAcceptRequest = new Button();
            btnAcceptRequest.Text = "接受请求";
            btnAcceptRequest.Size = new Size(80, 30);
            btnAcceptRequest.Location = new Point(550, 370);
            btnAcceptRequest.Click += BtnAcceptRequest_Click;
            
            // 拒绝请求按钮
            Button btnRejectRequest = new Button();
            btnRejectRequest.Text = "拒绝请求";
            btnRejectRequest.Size = new Size(80, 30);
            btnRejectRequest.Location = new Point(650, 370);
            btnRejectRequest.Click += BtnRejectRequest_Click;
            
            tabPageRequests.Controls.Add(dgvRequests);
            tabPageRequests.Controls.Add(btnAcceptRequest);
            tabPageRequests.Controls.Add(btnRejectRequest);
            
            // 添加好友标签页
            TabPage tabPageAdd = new TabPage("添加好友");
            tabPageAdd.Name = "tabPageAdd";
            
            // 添加好友控件
            Label lblAddFriend = new Label();
            lblAddFriend.Text = "请输入要添加的好友用户名:";
            lblAddFriend.AutoSize = true;
            lblAddFriend.Location = new Point(50, 50);
            
            TextBox txtFriendUsername = new TextBox();
            txtFriendUsername.Name = "txtFriendUsername";
            txtFriendUsername.Size = new Size(200, 25);
            txtFriendUsername.Location = new Point(250, 48);
            
            Button btnAddFriend = new Button();
            btnAddFriend.Text = "添加好友";
            btnAddFriend.Size = new Size(80, 30);
            btnAddFriend.Location = new Point(480, 45);
            btnAddFriend.Click += BtnAddFriend_Click;
            
            Label lblAddResult = new Label();
            lblAddResult.Name = "lblAddResult";
            lblAddResult.AutoSize = true;
            lblAddResult.Location = new Point(50, 100);
            lblAddResult.ForeColor = Color.Green;
            
            tabPageAdd.Controls.Add(lblAddFriend);
            tabPageAdd.Controls.Add(txtFriendUsername);
            tabPageAdd.Controls.Add(btnAddFriend);
            tabPageAdd.Controls.Add(lblAddResult);
            
            // 刷新按钮
            Button btnRefresh = new Button();
            btnRefresh.Text = "刷新";
            btnRefresh.Size = new Size(80, 30);
            btnRefresh.Location = new Point(700, 540);
            btnRefresh.Click += BtnRefresh_Click;
            
            // 添加标签页到标签控件
            tabControl.TabPages.Add(tabPageFriends);
            tabControl.TabPages.Add(tabPageRequests);
            tabControl.TabPages.Add(tabPageAdd);
            
            // 添加控件到窗体
            this.Controls.Add(lblTitle);
            this.Controls.Add(tabControl);
            this.Controls.Add(btnRefresh);
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadFriends()
        {
            try
            {
                friends = FriendDAO.GetFriendsByUserId(currentUser.UserId);
                pendingFriends = FriendDAO.GetPendingFriendsByUserId(currentUser.UserId);
                
                // 更新好友列表
                DataGridView dgvFriends = this.Controls.Find("tabPageFriends", true)
                    .FirstOrDefault()?.Controls.Find("dgvFriends", true)
                    .FirstOrDefault() as DataGridView;
                
                if (dgvFriends != null)
                {
                    var bindingSource = new BindingSource();
                    bindingSource.DataSource = friends.Select(f => new {
                        Username = f.Username,
                        Email = f.Email,
                        LastLogin = f.LastLogin.ToString("yyyy-MM-dd HH:mm")
                    }).ToList();
                    
                    dgvFriends.DataSource = bindingSource;
                }
                
                // 更新好友请求列表
                DataGridView dgvRequests = this.Controls.Find("tabPageRequests", true)
                    .FirstOrDefault()?.Controls.Find("dgvRequests", true)
                    .FirstOrDefault() as DataGridView;
                
                if (dgvRequests != null)
                {
                    var bindingSource = new BindingSource();
                    bindingSource.DataSource = pendingFriends.Select(f => new {
                        Username = f.Username,
                        Email = f.Email,
                        CreatedAt = f.CreatedAt.ToString("yyyy-MM-dd HH:mm")
                    }).ToList();
                    
                    dgvRequests.DataSource = bindingSource;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载好友列表失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDeleteFriend_Click(object? sender, EventArgs e)
        {
            DataGridView dgvFriends = this.Controls.Find("tabPageFriends", true)
                .FirstOrDefault()?.Controls.Find("dgvFriends", true)
                .FirstOrDefault() as DataGridView;
            
            if (dgvFriends?.SelectedRows?.Count > 0)
            {
                var selectedRow = dgvFriends.SelectedRows[0];
                string username = selectedRow.Cells["Username"].Value.ToString();
                
                // 查找选中的好友
                var selectedFriend = friends.FirstOrDefault(f => f.Username == username);
                if (selectedFriend != null)
                {
                    var result = MessageBox.Show($"确定要删除好友 {username} 吗?", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            if (FriendDAO.DeleteFriend(currentUser.UserId, selectedFriend.UserId))
                            {
                                MessageBox.Show("好友删除成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadFriends();
                            }
                            else
                            {
                                MessageBox.Show("好友删除失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("请先选择要删除的好友", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnAcceptRequest_Click(object? sender, EventArgs e)
        {
            DataGridView dgvRequests = this.Controls.Find("tabPageRequests", true)
                .FirstOrDefault()?.Controls.Find("dgvRequests", true)
                .FirstOrDefault() as DataGridView;
            
            if (dgvRequests?.SelectedRows?.Count > 0)
            {
                var selectedRow = dgvRequests.SelectedRows[0];
                string username = selectedRow.Cells["RequestUsername"].Value.ToString();
                
                // 查找选中的请求
                var selectedRequest = pendingFriends.FirstOrDefault(f => f.Username == username);
                if (selectedRequest != null)
                {
                    try
                    {
                        if (FriendDAO.ConfirmFriend(currentUser.UserId, selectedRequest.UserId))
                        {
                            MessageBox.Show("好友请求已接受", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadFriends();
                        }
                        else
                        {
                            MessageBox.Show("接受好友请求失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"操作失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("请先选择要接受的好友请求", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnRejectRequest_Click(object? sender, EventArgs e)
        {
            DataGridView dgvRequests = this.Controls.Find("tabPageRequests", true)
                .FirstOrDefault()?.Controls.Find("dgvRequests", true)
                .FirstOrDefault() as DataGridView;
            
            if (dgvRequests?.SelectedRows?.Count > 0)
            {
                var selectedRow = dgvRequests.SelectedRows[0];
                string username = selectedRow.Cells["RequestUsername"].Value.ToString();
                
                // 查找选中的请求
                var selectedRequest = pendingFriends.FirstOrDefault(f => f.Username == username);
                if (selectedRequest != null)
                {
                    var result = MessageBox.Show($"确定要拒绝 {username} 的好友请求吗?", "确认拒绝", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            if (FriendDAO.DeleteFriend(currentUser.UserId, selectedRequest.UserId))
                            {
                                MessageBox.Show("好友请求已拒绝", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadFriends();
                            }
                            else
                            {
                                MessageBox.Show("拒绝好友请求失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"操作失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("请先选择要拒绝的好友请求", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnAddFriend_Click(object? sender, EventArgs e)
        {
            TextBox txtFriendUsername = this.Controls.Find("tabPageAdd", true)
                .FirstOrDefault()?.Controls.Find("txtFriendUsername", true)
                .FirstOrDefault() as TextBox;
            
            Label lblAddResult = this.Controls.Find("tabPageAdd", true)
                .FirstOrDefault()?.Controls.Find("lblAddResult", true)
                .FirstOrDefault() as Label;
            
            if (string.IsNullOrWhiteSpace(txtFriendUsername?.Text))
            {
                MessageBox.Show("请输入好友用户名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // 检查是否是自己
            if (txtFriendUsername.Text == currentUser.Username)
            {
                lblAddResult.Text = "不能添加自己为好友";
                lblAddResult.ForeColor = Color.Red;
                return;
            }
            
            try
            {
                // 查找用户
                var friend = UserDAO.GetUserByUsername(txtFriendUsername.Text);
                if (friend == null)
                {
                    lblAddResult.Text = "用户不存在";
                    lblAddResult.ForeColor = Color.Red;
                    return;
                }
                
                // 检查是否已经是好友
                if (friends.Any(f => f.UserId == friend.UserId))
                {
                    lblAddResult.Text = "该用户已经是您的好友";
                    lblAddResult.ForeColor = Color.Red;
                    return;
                }
                
                // 添加好友请求
                if (FriendDAO.AddFriend(currentUser.UserId, friend.UserId))
                {
                    lblAddResult.Text = "好友请求已发送";
                    lblAddResult.ForeColor = Color.Green;
                    txtFriendUsername.Text = "";
                }
                else
                {
                    lblAddResult.Text = "发送好友请求失败";
                    lblAddResult.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblAddResult.Text = $"操作失败: {ex.Message}";
                lblAddResult.ForeColor = Color.Red;
            }
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadFriends();
        }
    }
}