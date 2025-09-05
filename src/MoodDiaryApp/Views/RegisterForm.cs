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

namespace MoodDiaryApp.Views
{
    public partial class RegisterForm : Form
    {
        public string RegisteredUsername { get; private set; }
        
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // 设置窗体属性
            this.Text = "心情树洞 - 用户注册";
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            // 创建控件
            Label lblTitle = new Label();
            lblTitle.Text = "用户注册";
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
            
            Label lblConfirmPassword = new Label();
            lblConfirmPassword.Text = "确认密码:";
            lblConfirmPassword.AutoSize = true;
            lblConfirmPassword.Location = new Point(50, 160);
            
            TextBox txtConfirmPassword = new TextBox();
            txtConfirmPassword.Name = "txtConfirmPassword";
            txtConfirmPassword.Size = new Size(200, 25);
            txtConfirmPassword.Location = new Point(120, 158);
            txtConfirmPassword.UseSystemPasswordChar = true;
            
            Label lblEmail = new Label();
            lblEmail.Text = "邮箱:";
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(50, 200);
            
            TextBox txtEmail = new TextBox();
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(200, 25);
            txtEmail.Location = new Point(120, 198);
            
            Button btnRegister = new Button();
            btnRegister.Text = "注册";
            btnRegister.Size = new Size(80, 30);
            btnRegister.Location = new Point(100, 250);
            btnRegister.Click += BtnRegister_Click;
            
            Button btnCancel = new Button();
            btnCancel.Text = "取消";
            btnCancel.Size = new Size(80, 30);
            btnCancel.Location = new Point(200, 250);
            btnCancel.Click += BtnCancel_Click;
            
            // 添加控件到窗体
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(lblConfirmPassword);
            this.Controls.Add(txtConfirmPassword);
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(btnRegister);
            this.Controls.Add(btnCancel);
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            TextBox txtUsername = this.Controls.Find("txtUsername", true).FirstOrDefault() as TextBox;
            TextBox txtPassword = this.Controls.Find("txtPassword", true).FirstOrDefault() as TextBox;
            TextBox txtConfirmPassword = this.Controls.Find("txtConfirmPassword", true).FirstOrDefault() as TextBox;
            TextBox txtEmail = this.Controls.Find("txtEmail", true).FirstOrDefault() as TextBox;
            
            // 验证输入
            if (string.IsNullOrWhiteSpace(txtUsername?.Text))
            {
                MessageBox.Show("请输入用户名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(txtPassword?.Text))
            {
                MessageBox.Show("请输入密码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (txtPassword.Text != txtConfirmPassword?.Text)
            {
                MessageBox.Show("两次输入的密码不一致", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            var (success, message, userId) = AuthenticationService.Register(txtUsername.Text, txtPassword.Text, txtEmail?.Text);
            
            if (success)
            {
                RegisteredUsername = txtUsername.Text;
                MessageBox.Show(message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(message, "注册失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}