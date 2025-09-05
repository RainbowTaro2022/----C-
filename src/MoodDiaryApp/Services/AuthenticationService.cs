using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using MoodDiaryApp.Models;
using MoodDiaryApp.Data;

namespace MoodDiaryApp.Services
{
    public class AuthenticationService
    {
        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="email">邮箱</param>
        /// <returns>注册结果</returns>
        public static (bool success, string message, int userId) Register(string username, string password, string email)
        {
            try
            {
                // 检查用户名是否已存在
                if (UserDAO.GetUserByUsername(username) != null)
                {
                    return (false, "用户名已存在", 0);
                }

                // 创建新用户
                var user = new User
                {
                    Username = username,
                    Password = HashPassword(password),
                    Email = email
                };

                int userId = UserDAO.AddUser(user);
                return (userId > 0, userId > 0 ? "注册成功" : "注册失败", userId);
            }
            catch (Exception ex)
            {
                return (false, $"注册失败: {ex.Message}", 0);
            }
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>登录结果</returns>
        public static (bool success, string message, User? user) Login(string username, string password)
        {
            try
            {
                // 获取用户信息
                var user = UserDAO.GetUserByUsername(username);
                if (user == null)
                {
                    return (false, "用户不存在", null);
                }

                // 验证密码
                if (!VerifyPassword(password, user.Password))
                {
                    return (false, "密码错误", null);
                }

                // 更新最后登录时间
                user.LastLogin = DateTime.Now;
                UserDAO.UpdateUser(user);

                return (true, "登录成功", user);
            }
            catch (Exception ex)
            {
                return (false, $"登录失败: {ex.Message}", null);
            }
        }

        /// <summary>
        /// 密码哈希
        /// </summary>
        /// <param name="password">原始密码</param>
        /// <returns>哈希后的密码</returns>
        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        /// <summary>
        /// 验证密码
        /// </summary>
        /// <param name="password">输入的密码</param>
        /// <param name="hashedPassword">存储的哈希密码</param>
        /// <returns>是否匹配</returns>
        private static bool VerifyPassword(string password, string hashedPassword)
        {
            string hashedInput = HashPassword(password);
            return hashedInput.Equals(hashedPassword);
        }
    }
}