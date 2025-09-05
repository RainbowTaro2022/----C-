using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Xunit;
using MoodDiaryApp.Models;
using MoodDiaryApp.Data;

namespace MoodDiary.Tests
{
    public class PerformanceTests : IDisposable
    {
        private string testDbPath;
        
        public PerformanceTests()
        {
            // 设置测试数据库路径
            testDbPath = Path.Combine(Path.GetTempPath(), "MoodDiaryPerformanceTest.db");
            
            // 设置环境变量以使用测试数据库
            Environment.SetEnvironmentVariable("MOOD_DIARY_DB_PATH", testDbPath);
            
            // 删除现有的测试数据库文件（如果存在）
            if (File.Exists(testDbPath))
            {
                File.Delete(testDbPath);
            }
            
            // 重新初始化数据库
            DatabaseHelper.ReinitializeDatabase();
        }
        
        public void Dispose()
        {
            // 清理测试数据库文件
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
            
            Environment.SetEnvironmentVariable("MOOD_DIARY_DB_PATH", null);
        }
        
        [Fact]
        public void UserDAO_AddUser_PerformanceTest()
        {
            // Arrange
            var stopwatch = new Stopwatch();
            var users = new List<User>();
            
            // 创建100个用户对象
            for (int i = 0; i < 100; i++)
            {
                users.Add(new User
                {
                    Username = $"testuser{i}",
                    Password = $"testpassword{i}",
                    Email = $"test{i}@example.com"
                });
            }
            
            // Act
            stopwatch.Start();
            foreach (var user in users)
            {
                UserDAO.AddUser(user);
            }
            stopwatch.Stop();
            
            // Assert
            // 100个用户插入应该在4秒内完成（在CI/CD环境中可能需要更多时间）
            Assert.True(stopwatch.ElapsedMilliseconds < 4000, 
                $"插入100个用户耗时 {stopwatch.ElapsedMilliseconds} 毫秒，超过了4秒的预期");
        }
        
        [Fact]
        public void UserDAO_GetUserById_PerformanceTest()
        {
            // Arrange
            // 首先添加一个用户
            var user = new User
            {
                Username = "testuser_perf",
                Password = "testpassword_perf",
                Email = "test_perf@example.com"
            };
            
            int userId = UserDAO.AddUser(user);
            var stopwatch = new Stopwatch();
            
            // Act
            stopwatch.Start();
            for (int i = 0; i < 100; i++)
            {
                var retrievedUser = UserDAO.GetUserById(userId);
                Assert.NotNull(retrievedUser);
            }
            stopwatch.Stop();
            
            // Assert
            // 100次查询应该在2秒内完成（在CI/CD环境中可能需要更多时间）
            Assert.True(stopwatch.ElapsedMilliseconds < 2000, 
                $"查询100次用户信息耗时 {stopwatch.ElapsedMilliseconds} 毫秒，超过了2秒的预期");
        }
        
        [Fact]
        public void MoodRecordDAO_AddMoodRecord_PerformanceTest()
        {
            // Arrange
            // 首先添加一个用户
            var user = new User
            {
                Username = "testuser_perf2",
                Password = "testpassword_perf2",
                Email = "test_perf2@example.com"
            };
            
            int userId = UserDAO.AddUser(user);
            
            var stopwatch = new Stopwatch();
            var records = new List<MoodRecord>();
            
            // 创建100个心情记录对象
            for (int i = 0; i < 100; i++)
            {
                records.Add(new MoodRecord
                {
                    UserId = userId,
                    RecordDate = DateTime.Now.AddDays(-i),
                    MoodText = $"心情记录 {i}",
                    MoodScore = i % 5,
                    PrivacyMode = 1,
                    Location = $"地点 {i}"
                });
            }
            
            // Act
            stopwatch.Start();
            foreach (var record in records)
            {
                MoodRecordDAO.AddMoodRecord(record);
            }
            stopwatch.Stop();
            
            // Assert
            // 100个心情记录插入应该在5秒内完成（在CI/CD环境中可能需要更多时间）
            Assert.True(stopwatch.ElapsedMilliseconds < 5000, 
                $"插入100个心情记录耗时 {stopwatch.ElapsedMilliseconds} 毫秒，超过了5秒的预期");
        }
        
        [Fact]
        public void MoodRecordDAO_GetMoodRecordsByUserId_PerformanceTest()
        {
            // Arrange
            // 首先添加一个用户
            var user = new User
            {
                Username = "testuser_perf3",
                Password = "testpassword_perf3",
                Email = "test_perf3@example.com"
            };
            
            int userId = UserDAO.AddUser(user);
            
            // 添加100条心情记录
            for (int i = 0; i < 100; i++)
            {
                var moodRecord = new MoodRecord
                {
                    UserId = userId,
                    RecordDate = DateTime.Now.AddDays(-i),
                    MoodText = $"心情记录 {i}",
                    MoodScore = i % 5,
                    PrivacyMode = 1,
                    Location = $"地点 {i}"
                };
                
                MoodRecordDAO.AddMoodRecord(moodRecord);
            }
            
            var stopwatch = new Stopwatch();
            
            // Act
            stopwatch.Start();
            for (int i = 0; i < 10; i++)
            {
                var records = MoodRecordDAO.GetMoodRecordsByUserId(userId);
                Assert.Equal(100, records.Count);
            }
            stopwatch.Stop();
            
            // Assert
            // 10次查询100条记录应该在2秒内完成（在CI/CD环境中可能需要更多时间）
            Assert.True(stopwatch.ElapsedMilliseconds < 2000, 
                $"查询10次心情记录列表耗时 {stopwatch.ElapsedMilliseconds} 毫秒，超过了2秒的预期");
        }
    }
}