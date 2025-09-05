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
    public partial class AccountSettingsForm : Form
    {
        private User currentUser;
        
        public AccountSettingsForm(User user)
        {
            currentUser = user;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // 设置窗体属性
            this.Text = "账户设置";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            // 创建控件
            Label lblTitle = new Label();
            lblTitle.Text = "账户设置";
            lblTitle.Font = new Font("微软雅黑", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.Purple;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(180, 20);
            
            // 用户名标签和文本框
            Label lblUsername = new Label();
            lblUsername.Text = "用户名:";
            lblUsername.AutoSize = true;
            lblUsername.Location = new Point(50, 80);
            
            TextBox txtUsername = new TextBox();
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(200, 25);
            txtUsername.Location = new Point(150, 78);
            txtUsername.Text = currentUser.Username;
            
            // 邮箱标签和文本框
            Label lblEmail = new Label();
            lblEmail.Text = "邮箱:";
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(50, 120);
            
            TextBox txtEmail = new TextBox();
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(200, 25);
            txtEmail.Location = new Point(150, 118);
            txtEmail.Text = currentUser.Email ?? "";
            
            // 当前密码标签和文本框
            Label lblCurrentPassword = new Label();
            lblCurrentPassword.Text = "当前密码:";
            lblCurrentPassword.AutoSize = true;
            lblCurrentPassword.Location = new Point(50, 160);
            
            TextBox txtCurrentPassword = new TextBox();
            txtCurrentPassword.Name = "txtCurrentPassword";
            txtCurrentPassword.Size = new Size(200, 25);
            txtCurrentPassword.Location = new Point(150, 158);
            txtCurrentPassword.UseSystemPasswordChar = true;
            
            // 新密码标签和文本框
            Label lblNewPassword = new Label();
            lblNewPassword.Text = "新密码:";
            lblNewPassword.AutoSize = true;
            lblNewPassword.Location = new Point(50, 200);
            
            TextBox txtNewPassword = new TextBox();
            txtNewPassword.Name = "txtNewPassword";
            txtNewPassword.Size = new Size(200, 25);
            txtNewPassword.Location = new Point(150, 198);
            txtNewPassword.UseSystemPasswordChar = true;
            
            // 确认密码标签和文本框
            Label lblConfirmPassword = new Label();
            lblConfirmPassword.Text = "确认密码:";
            lblConfirmPassword.AutoSize = true;
            lblConfirmPassword.Location = new Point(50, 240);
            
            TextBox txtConfirmPassword = new TextBox();
            txtConfirmPassword.Name = "txtConfirmPassword";
            txtConfirmPassword.Size = new Size(200, 25);
            txtConfirmPassword.Location = new Point(150, 238);
            txtConfirmPassword.UseSystemPasswordChar = true;
            
            // 保存按钮
            Button btnSave = new Button();
            btnSave.Text = "保存";
            btnSave.Size = new Size(80, 30);
            btnSave.Location = new Point(150, 280);
            btnSave.Click += BtnSave_Click;
            
            // 取消按钮
            Button btnCancel = new Button();
            btnCancel.Text = "取消";
            btnCancel.Size = new Size(80, 30);
            btnCancel.Location = new Point(250, 280);
            btnCancel.Click += BtnCancel_Click;
            
            // 添加控件到窗体
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(lblCurrentPassword);
            this.Controls.Add(txtCurrentPassword);
            this.Controls.Add(lblNewPassword);
            this.Controls.Add(txtNewPassword);
            this.Controls.Add(lblConfirmPassword);
            this.Controls.Add(txtConfirmPassword);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            // 获取控件
            TextBox txtUsername = this.Controls.Find("txtUsername", true).FirstOrDefault() as TextBox;
            TextBox txtEmail = this.Controls.Find("txtEmail", true).FirstOrDefault() as TextBox;
            TextBox txtCurrentPassword = this.Controls.Find("txtCurrentPassword", true).FirstOrDefault() as TextBox;
            TextBox txtNewPassword = this.Controls.Find("txtNewPassword", true).FirstOrDefault() as TextBox;
            TextBox txtConfirmPassword = this.Controls.Find("txtConfirmPassword", true).FirstOrDefault() as TextBox;
            
            // 验证输入
            if (string.IsNullOrWhiteSpace(txtUsername?.Text))
            {
                MessageBox.Show("用户名不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // 检查用户名是否已更改
            if (txtUsername.Text != currentUser.Username)
            {
                // 检查新用户名是否已存在
                var existingUser = UserDAO.GetUserByUsername(txtUsername.Text);
                if (existingUser != null && existingUser.UserId != currentUser.UserId)
                {
                    MessageBox.Show("用户名已存在，请选择其他用户名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            
            // 验证邮箱格式（如果填写了）
            if (!string.IsNullOrWhiteSpace(txtEmail?.Text))
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(txtEmail.Text);
                    if (addr.Address != txtEmail.Text)
                    {
                        MessageBox.Show("邮箱格式不正确", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("邮箱格式不正确", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            
            // 检查密码更改
            if (!string.IsNullOrWhiteSpace(txtNewPassword?.Text) || !string.IsNullOrWhiteSpace(txtConfirmPassword?.Text))
            {
                // 验证当前密码
                if (string.IsNullOrWhiteSpace(txtCurrentPassword?.Text))
                {
                    MessageBox.Show("请输入当前密码以更改密码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // 验证当前密码是否正确
                if (txtCurrentPassword.Text != currentUser.Password)
                {
                    MessageBox.Show("当前密码不正确", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // 验证新密码和确认密码
                if (txtNewPassword?.Text != txtConfirmPassword?.Text)
                {
                    MessageBox.Show("新密码和确认密码不匹配", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // 更新密码
                currentUser.Password = txtNewPassword.Text;
            }
            
            // 更新用户信息
            currentUser.Username = txtUsername.Text;
            currentUser.Email = string.IsNullOrWhiteSpace(txtEmail?.Text) ? null : txtEmail.Text;
            
            // 保存到数据库
            try
            {
                if (UserDAO.UpdateUser(currentUser))
                {
                    MessageBox.Show("账户信息保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("保存失败，请重试", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}