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
    public partial class PrivacySettingsForm : Form
    {
        private User currentUser;
        
        public PrivacySettingsForm(User user)
        {
            currentUser = user;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // 设置窗体属性
            this.Text = "隐私设置";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            // 创建控件
            Label lblTitle = new Label();
            lblTitle.Text = "隐私设置";
            lblTitle.Font = new Font("微软雅黑", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.Purple;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(180, 20);
            
            // 默认隐私模式标签
            Label lblDefaultPrivacy = new Label();
            lblDefaultPrivacy.Text = "默认隐私模式:";
            lblDefaultPrivacy.AutoSize = true;
            lblDefaultPrivacy.Location = new Point(50, 80);
            
            // 默认隐私模式组合框
            ComboBox cmbDefaultPrivacy = new ComboBox();
            cmbDefaultPrivacy.Name = "cmbDefaultPrivacy";
            cmbDefaultPrivacy.Size = new Size(200, 25);
            cmbDefaultPrivacy.Location = new Point(150, 78);
            cmbDefaultPrivacy.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDefaultPrivacy.Items.Add("匿名模式");
            cmbDefaultPrivacy.Items.Add("实名模式");
            cmbDefaultPrivacy.SelectedIndex = currentUser.DefaultPrivacyMode; // 0-匿名，1-实名
            
            // 说明标签
            Label lblExplanation = new Label();
            lblExplanation.Text = "说明：\n匿名模式 - 您的心情记录将不显示您的身份信息\n实名模式 - 您的心情记录将显示您的用户名";
            lblExplanation.AutoSize = true;
            lblExplanation.Location = new Point(50, 120);
            lblExplanation.ForeColor = Color.Gray;
            
            // 保存按钮
            Button btnSave = new Button();
            btnSave.Text = "保存";
            btnSave.Size = new Size(80, 30);
            btnSave.Location = new Point(150, 200);
            btnSave.Click += BtnSave_Click;
            
            // 取消按钮
            Button btnCancel = new Button();
            btnCancel.Text = "取消";
            btnCancel.Size = new Size(80, 30);
            btnCancel.Location = new Point(250, 200);
            btnCancel.Click += BtnCancel_Click;
            
            // 添加控件到窗体
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblDefaultPrivacy);
            this.Controls.Add(cmbDefaultPrivacy);
            this.Controls.Add(lblExplanation);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            // 获取控件
            ComboBox cmbDefaultPrivacy = this.Controls.Find("cmbDefaultPrivacy", true).FirstOrDefault() as ComboBox;
            
            // 更新用户信息
            currentUser.DefaultPrivacyMode = cmbDefaultPrivacy.SelectedIndex;
            
            // 保存到数据库
            try
            {
                if (UserDAO.UpdateUser(currentUser))
                {
                    MessageBox.Show("隐私设置保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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