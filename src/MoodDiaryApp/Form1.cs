using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoodDiaryApp.Views;
using MoodDiaryApp.Models;

namespace MoodDiaryApp
{
    public partial class Form1 : Form
    {
        private User currentUser;
        private System.ComponentModel.IContainer components = null;

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(User user)
        {
            currentUser = user;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Text = "心情树洞";
            this.IsMdiContainer = true;
            
            // 创建主菜单
            CreateMainMenu();
        }

        private void CreateMainMenu()
        {
            var menuStrip = new MenuStrip();
            
            // 日记菜单
            var diaryMenu = new ToolStripMenuItem("心情日记");
            diaryMenu.DropDownItems.Add("日历视图", null, (s, e) => ShowCalendarViewForm());
            diaryMenu.DropDownItems.Add("添加日记", null, (s, e) => ShowAddDiaryForm());
            diaryMenu.DropDownItems.Add("查看日记", null, (s, e) => ShowViewDiaryForm());
            diaryMenu.DropDownItems.Add("树形视图", null, (s, e) => ShowMoodTreeForm());
            menuStrip.Items.Add(diaryMenu);
            
            // 统计菜单
            var statsMenu = new ToolStripMenuItem("统计分析");
            statsMenu.DropDownItems.Add("情绪趋势", null, (s, e) => ShowMoodTrendForm());
            statsMenu.DropDownItems.Add("情绪分布", null, (s, e) => ShowMoodDistributionForm());
            menuStrip.Items.Add(statsMenu);
            
            // 社交菜单
            var socialMenu = new ToolStripMenuItem("好友互动");
            socialMenu.DropDownItems.Add("好友列表", null, (s, e) => ShowFriendsForm());
            socialMenu.DropDownItems.Add("消息通知", null, (s, e) => ShowNotificationsForm());
            menuStrip.Items.Add(socialMenu);
            
            // 设置菜单
            var settingsMenu = new ToolStripMenuItem("个人设置");
            settingsMenu.DropDownItems.Add("账户设置", null, (s, e) => ShowAccountSettingsForm());
            settingsMenu.DropDownItems.Add("隐私设置", null, (s, e) => ShowPrivacySettingsForm());
            settingsMenu.DropDownItems.Add("表情包管理", null, (s, e) => ShowEmojiManagerForm());
            settingsMenu.DropDownItems.Add("云同步管理", null, (s, e) => ShowCloudSyncForm());
            menuStrip.Items.Add(settingsMenu);
            
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        private void ShowCalendarViewForm()
        {
            if (currentUser != null)
            {
                using (var form = new CalendarViewForm(currentUser))
                {
                    form.ShowDialog();
                }
            }
        }

        private void ShowAddDiaryForm()
        {
            if (currentUser != null)
            {
                using (var form = new AddDiaryForm(currentUser))
                {
                    form.ShowDialog();
                }
            }
        }

        private void ShowViewDiaryForm()
        {
            if (currentUser != null)
            {
                using (var form = new ViewDiaryForm(currentUser))
                {
                    form.ShowDialog();
                }
            }
        }

        private void ShowMoodTrendForm()
        {
            if (currentUser != null)
            {
                using (var form = new MoodTrendForm(currentUser))
                {
                    form.ShowDialog();
                }
            }
        }

        private void ShowMoodDistributionForm()
        {
            if (currentUser != null)
            {
                using (var form = new MoodDistributionForm(currentUser))
                {
                    form.ShowDialog();
                }
            }
        }

        private void ShowFriendsForm()
        {
            if (currentUser != null)
            {
                using (var form = new FriendsForm(currentUser))
                {
                    form.ShowDialog();
                }
            }
        }

        private void ShowNotificationsForm()
        {
            if (currentUser != null)
            {
                using (var form = new NotificationsForm(currentUser))
                {
                    form.ShowDialog();
                }
            }
        }

        private void ShowMoodTreeForm()
        {
            if (currentUser != null)
            {
                using (var form = new MoodTreeForm(currentUser))
                {
                    form.ShowDialog();
                }
            }
        }

        private void ShowEmojiManagerForm()
        {
            if (currentUser != null)
            {
                using (var form = new EmojiManagerForm(currentUser))
                {
                    form.ShowDialog();
                }
            }
        }

        private void ShowCloudSyncForm()
        {
            if (currentUser != null)
            {
                using (var form = new CloudSyncForm(currentUser))
                {
                    form.ShowDialog();
                }
            }
        }

        private void ShowAccountSettingsForm()
        {
            if (currentUser != null)
            {
                using (var form = new AccountSettingsForm(currentUser))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        // 如果账户信息被修改，可能需要更新主窗体中的用户信息
                        // 这里可以添加刷新用户信息的逻辑
                        MessageBox.Show("账户信息已更新", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void ShowPrivacySettingsForm()
        {
            if (currentUser != null)
            {
                using (var form = new PrivacySettingsForm(currentUser))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        // 如果隐私设置被修改，可能需要更新主窗体中的用户信息
                        MessageBox.Show("隐私设置已更新", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
    }
}