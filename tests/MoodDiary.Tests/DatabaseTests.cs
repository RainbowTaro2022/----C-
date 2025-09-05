using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Xunit;
using MoodDiaryApp.Models;
using MoodDiaryApp.Data;

namespace MoodDiary.Tests
{
    public class DatabaseTests : IDisposable
    {
        private string testDbPath;
        
        public DatabaseTests()
        {
            // 设置测试数据库路径
            testDbPath = Path.Combine(Path.GetTempPath(), $"MoodDiaryTest_{Guid.NewGuid()}.db");
            
            // 设置环境变量以使用测试数据库
            Environment.SetEnvironmentVariable("MOOD_DIARY_DB_PATH", testDbPath);
            
            // 确保数据库文件不存在
            if (File.Exists(testDbPath))
            {
                // 尝试删除文件，如果失败则等待一段时间再试
                try
                {
                    File.Delete(testDbPath);
                }
                catch (IOException)
                {
                    Thread.Sleep(100);
                    try
                    {
                        File.Delete(testDbPath);
                    }
                    catch
                    {
                        // 如果仍然失败，使用新的文件名
                        testDbPath = Path.Combine(Path.GetTempPath(), $"MoodDiaryTest_{Guid.NewGuid()}.db");
                        Environment.SetEnvironmentVariable("MOOD_DIARY_DB_PATH", testDbPath);
                    }
                }
            }
            
            // 重新初始化数据库
            DatabaseHelper.ReinitializeDatabase();
        }
        
        public void Dispose()
        {
            // 清理测试数据库文件
            Environment.SetEnvironmentVariable("MOOD_DIARY_DB_PATH", null);
            
            if (File.Exists(testDbPath))
            {
                try
                {
                    File.Delete(testDbPath);
                }
                catch
                {
                    // 忽略删除失败的情况
                }
            }
        }
        
        [Fact]
        public void UserDAO_AddUser_ReturnsValidId()
        {
            // Arrange
            var user = new User
            {
                Username = "testuser_" + Guid.NewGuid().ToString("N")[..8],
                Password = "testpassword",
                Email = "test@example.com"
            };
            
            // Act
            int userId = UserDAO.AddUser(user);
            
            // Assert
            Assert.True(userId > 0);
        }
        
        [Fact]
        public void UserDAO_GetUserById_ReturnsCorrectUser()
        {
            // Arrange
            var user = new User
            {
                Username = "testuser2_" + Guid.NewGuid().ToString("N")[..8],
                Password = "testpassword2",
                Email = "test2@example.com"
            };
            
            int userId = UserDAO.AddUser(user);
            
            // Act
            var retrievedUser = UserDAO.GetUserById(userId);
            
            // Assert
            Assert.NotNull(retrievedUser);
            Assert.Equal(userId, retrievedUser.UserId);
            Assert.Equal(user.Username, retrievedUser.Username);
            Assert.Equal("testpassword2", retrievedUser.Password);
            Assert.Equal("test2@example.com", retrievedUser.Email);
        }
        
        [Fact]
        public void MoodRecordDAO_AddMoodRecord_ReturnsValidId()
        {
            // Arrange
            // 首先添加一个用户
            var user = new User
            {
                Username = "testuser3_" + Guid.NewGuid().ToString("N")[..8],
                Password = "testpassword3",
                Email = "test3@example.com"
            };
            
            int userId = UserDAO.AddUser(user);
            
            var moodRecord = new MoodRecord
            {
                UserId = userId,
                RecordDate = DateTime.Now,
                MoodText = "今天心情很好",
                MoodScore = 2,
                PrivacyMode = 1,
                Location = "家里"
            };
            
            // Act
            int recordId = MoodRecordDAO.AddMoodRecord(moodRecord);
            
            // Assert
            Assert.True(recordId > 0);
        }
        
        [Fact]
        public void MoodRecordDAO_GetMoodRecordsByUserId_ReturnsCorrectRecords()
        {
            // Arrange
            // 首先添加一个用户
            var user = new User
            {
                Username = "testuser4_" + Guid.NewGuid().ToString("N")[..8],
                Password = "testpassword4",
                Email = "test4@example.com"
            };
            
            int userId = UserDAO.AddUser(user);
            
            // 添加几条心情记录
            var moodRecord1 = new MoodRecord
            {
                UserId = userId,
                RecordDate = DateTime.Now.AddDays(-1),
                MoodText = "昨天心情一般",
                MoodScore = 0,
                PrivacyMode = 1,
                Location = "公司"
            };
            
            var moodRecord2 = new MoodRecord
            {
                UserId = userId,
                RecordDate = DateTime.Now,
                MoodText = "今天心情很好",
                MoodScore = 2,
                PrivacyMode = 1,
                Location = "家里"
            };
            
            MoodRecordDAO.AddMoodRecord(moodRecord1);
            MoodRecordDAO.AddMoodRecord(moodRecord2);
            
            // Act
            var records = MoodRecordDAO.GetMoodRecordsByUserId(userId);
            
            // Assert
            Assert.NotNull(records);
            Assert.Equal(2, records.Count);
            Assert.Contains(records, r => r.MoodText == "今天心情很好");
            Assert.Contains(records, r => r.MoodText == "昨天心情一般");
        }
    }
}