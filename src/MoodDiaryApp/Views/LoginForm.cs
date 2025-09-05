using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoodDiaryApp.Services;
using MoodDiaryApp.Models;

namespace MoodDiaryApp.Views
{
    public partial class LoginForm : Form
    {
        public User LoggedInUser { get; private set; }
        
        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // 设置窗体属性
            this.Text = "心情树洞 - 用户登录";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            // 创建控件
            Label lblTitle = new Label();
            lblTitle.Text = "心情树洞";
            lblTitle.Font = new Font("微软雅黑", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.Purple;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(150, 30);
            
            Label lblUsername = new Label();
            lblUsername.Text = "用户名:";
            lblUsername.AutoSize = true;
            lblUsername.Location = new Point(50, 80);
            
            TextBox txtUsername = new TextBox();
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(200, 25);
            txtUsername.Location = new Point(120, 78);
            
            Label lblPassword = new Label();
            lblPassword.Text = "密码:";
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(50, 120);
            
            TextBox txtPassword = new TextBox();
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(200, 25);
            txtPassword.Location = new Point(120, 118);
            txtPassword.UseSystemPasswordChar = true;
            
            Button btnLogin = new Button();
            btnLogin.Text = "登录";
            btnLogin.Size = new Size(80, 30);
            btnLogin.Location = new Point(100, 170);
            btnLogin.Click += BtnLogin_Click;
            
            Button btnRegister = new Button();
            btnRegister.Text = "注册";
            btnRegister.Size = new Size(80, 30);
            btnRegister.Location = new Point(200, 170);
            btnRegister.Click += BtnRegister_Click;
            
            LinkLabel linkLblForgotPassword = new LinkLabel();
            linkLblForgotPassword.Text = "忘记密码?";
            linkLblForgotPassword.AutoSize = true;
            linkLblForgotPassword.Location = new Point(250, 122);
            linkLblForgotPassword.Click += LinkLblForgotPassword_Click;
            
            // 添加控件到窗体
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnRegister);
            this.Controls.Add(linkLblForgotPassword);
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            TextBox txtUsername = this.Controls.Find("txtUsername", true).FirstOrDefault() as TextBox;
            TextBox txtPassword = this.Controls.Find("txtPassword", true).FirstOrDefault() as TextBox;
            
            if (string.IsNullOrWhiteSpace(txtUsername?.Text) || string.IsNullOrWhiteSpace(txtPassword?.Text))
            {
                MessageBox.Show("请输入用户名和密码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            var (success, message, user) = AuthenticationService.Login(txtUsername.Text, txtPassword.Text);
            
            if (success)
            {
                LoggedInUser = user;
                MessageBox.Show(message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(message, "登录失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            using (var registerForm = new RegisterForm())
            {
                if (registerForm.ShowDialog() == DialogResult.OK)
                {
                    // 注册成功后自动填充用户名
                    TextBox txtUsername = this.Controls.Find("txtUsername", true).FirstOrDefault() as TextBox;
                    txtUsername.Text = registerForm.RegisteredUsername;
                }
            }
        }

        private void LinkLblForgotPassword_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("忘记密码功能待实现", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}