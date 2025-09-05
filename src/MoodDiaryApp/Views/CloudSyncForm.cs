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
using MoodDiaryApp.Services;
using System.IO;

namespace MoodDiaryApp.Views
{
    public partial class CloudSyncForm : Form
    {
        private User currentUser;
        private CloudSyncService cloudSyncService;
        private ListBox lstSyncFiles;
        
        public CloudSyncForm(User user)
        {
            currentUser = user;
            cloudSyncService = new CloudSyncService();
            InitializeComponent();
            LoadCloudSyncFiles();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // 设置窗体属性
            this.Text = "云同步管理";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            // 创建标题标签
            Label lblTitle = new Label();
            lblTitle.Text = "云同步管理";
            lblTitle.Font = new Font("微软雅黑", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.Purple;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(220, 20);
            
            // 创建说明标签
            Label lblInstruction = new Label();
            lblInstruction.Text = "您可以将心情数据导出到云端，或从云端导入数据";
            lblInstruction.AutoSize = true;
            lblInstruction.Location = new Point(50, 70);
            lblInstruction.ForeColor = Color.Gray;
            
            // 创建导出按钮
            Button btnExport = new Button();
            btnExport.Text = "导出到云端";
            btnExport.Size = new Size(120, 40);
            btnExport.Location = new Point(50, 120);
            btnExport.Click += BtnExport_Click;
            
            // 创建导入按钮
            Button btnImport = new Button();
            btnImport.Text = "从云端导入";
            btnImport.Size = new Size(120, 40);
            btnImport.Location = new Point(200, 120);
            btnImport.Click += BtnImport_Click;
            
            // 创建同步文件列表标签
            Label lblFiles = new Label();
            lblFiles.Text = "云端同步文件:";
            lblFiles.AutoSize = true;
            lblFiles.Location = new Point(50, 180);
            
            // 创建同步文件列表
            lstSyncFiles = new ListBox();
            lstSyncFiles.Name = "lstSyncFiles";
            lstSyncFiles.Size = new Size(480, 150);
            lstSyncFiles.Location = new Point(50, 200);
            
            // 创建删除按钮
            Button btnDelete = new Button();
            btnDelete.Text = "删除选中文件";
            btnDelete.Size = new Size(120, 30);
            btnDelete.Location = new Point(50, 370);
            btnDelete.Click += BtnDelete_Click;
            
            // 创建刷新按钮
            Button btnRefresh = new Button();
            btnRefresh.Text = "刷新列表";
            btnRefresh.Size = new Size(80, 30);
            btnRefresh.Location = new Point(180, 370);
            btnRefresh.Click += BtnRefresh_Click;
            
            // 添加控件到窗体
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblInstruction);
            this.Controls.Add(btnExport);
            this.Controls.Add(btnImport);
            this.Controls.Add(lblFiles);
            this.Controls.Add(lstSyncFiles);
            this.Controls.Add(btnDelete);
            this.Controls.Add(btnRefresh);
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadCloudSyncFiles()
        {
            lstSyncFiles.Items.Clear();
            var files = cloudSyncService.GetCloudSyncFiles();
            
            if (files.Count == 0)
            {
                lstSyncFiles.Items.Add("暂无云端同步文件");
                return;
            }
            
            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                lstSyncFiles.Items.Add(fileName);
            }
        }

        private void BtnExport_Click(object? sender, EventArgs e)
        {
            try
            {
                if (cloudSyncService.ExportToCloud(currentUser.UserId))
                {
                    MessageBox.Show("数据已成功导出到云端", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCloudSyncFiles(); // 刷新文件列表
                }
                else
                {
                    MessageBox.Show("数据导出失败，请重试", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"数据导出失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnImport_Click(object? sender, EventArgs e)
        {
            if (lstSyncFiles.SelectedItem == null || lstSyncFiles.SelectedItem.ToString() == "暂无云端同步文件")
            {
                MessageBox.Show("请先选择一个云端同步文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            string selectedFile = lstSyncFiles.SelectedItem.ToString();
            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                "MoodDiary", 
                "Cloud", 
                selectedFile);
            
            DialogResult result = MessageBox.Show(
                "确定要从云端导入数据吗？这将合并您的现有数据。", 
                "确认导入", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                try
                {
                    if (cloudSyncService.ImportFromCloud(currentUser.UserId, filePath))
                    {
                        MessageBox.Show("数据已成功从云端导入", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("数据导入失败，请重试", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"数据导入失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (lstSyncFiles.SelectedItem == null || lstSyncFiles.SelectedItem.ToString() == "暂无云端同步文件")
            {
                MessageBox.Show("请先选择一个云端同步文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            string selectedFile = lstSyncFiles.SelectedItem.ToString();
            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                "MoodDiary", 
                "Cloud", 
                selectedFile);
            
            DialogResult result = MessageBox.Show(
                $"确定要删除云端同步文件 '{selectedFile}' 吗？", 
                "确认删除", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                try
                {
                    if (cloudSyncService.DeleteCloudSyncFile(filePath))
                    {
                        MessageBox.Show("云端同步文件已删除", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadCloudSyncFiles(); // 刷新文件列表
                    }
                    else
                    {
                        MessageBox.Show("文件删除失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"文件删除失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadCloudSyncFiles();
        }
    }
}