using MoodDiaryApp.Views;
using MoodDiaryApp.Models;
using System;
using System.Windows.Forms;

namespace MoodDiaryApp;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        
        // 首先显示登录窗体
        User loggedInUser = null;
        using (var loginForm = new LoginForm())
        {
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                loggedInUser = loginForm.LoggedInUser;
            }
        }
        
        // 如果登录成功，显示主窗体
        if (loggedInUser != null)
        {
            Application.Run(new Form1(loggedInUser));
        }
    }    
}