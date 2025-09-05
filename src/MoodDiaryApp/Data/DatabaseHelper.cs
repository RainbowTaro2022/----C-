using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace MoodDiaryApp.Data
{
    public class DatabaseHelper
    {
        private static string dbPath = GetDatabasePath();
        private static string connectionString = $"Data Source={dbPath};Version=3;";

        private static string GetDatabasePath()
        {
            // 检查是否有测试环境变量
            string testDbPath = Environment.GetEnvironmentVariable("MOOD_DIARY_DB_PATH");
            if (!string.IsNullOrEmpty(testDbPath))
            {
                return testDbPath;
            }
            
            // 使用默认路径
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MoodDiary", "mooddiary.db");
        }

        static DatabaseHelper()
        {
            // 确保数据库目录存在
            string directory = Path.GetDirectoryName(dbPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 初始化数据库
            InitializeDatabase();
            // 更新表结构
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                UpdateTableStructure(connection);
            }
        }

        public static void ReinitializeDatabase()
        {
            // 重新初始化数据库
            InitializeDatabase();
        }

        private static void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // 创建用户表
                string createUserTable = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL UNIQUE,
                        Password TEXT NOT NULL,
                        Email TEXT,
                        DefaultPrivacyMode INTEGER DEFAULT 1,
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                        LastLogin DATETIME DEFAULT CURRENT_TIMESTAMP
                    )";
                ExecuteCommand(createUserTable, connection);

                // 创建心情记录表
                string createMoodRecordTable = @"
                    CREATE TABLE IF NOT EXISTS MoodRecords (
                        RecordId INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER NOT NULL,
                        RecordDate DATETIME NOT NULL,
                        MoodText TEXT,
                        MoodScore INTEGER,
                        PrivacyMode INTEGER DEFAULT 1,
                        Location TEXT,
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (UserId) REFERENCES Users(UserId)
                    )";
                ExecuteCommand(createMoodRecordTable, connection);

                // 创建情绪标签表
                string createMoodTagTable = @"
                    CREATE TABLE IF NOT EXISTS MoodTags (
                        TagId INTEGER PRIMARY KEY AUTOINCREMENT,
                        TagName TEXT NOT NULL UNIQUE,
                        TagCategory TEXT,
                        Color TEXT
                    )";
                ExecuteCommand(createMoodTagTable, connection);

                // 创建记录标签关联表
                string createRecordTagTable = @"
                    CREATE TABLE IF NOT EXISTS RecordTags (
                        RecordId INTEGER NOT NULL,
                        TagId INTEGER NOT NULL,
                        PRIMARY KEY (RecordId, TagId),
                        FOREIGN KEY (RecordId) REFERENCES MoodRecords(RecordId) ON DELETE CASCADE,
                        FOREIGN KEY (TagId) REFERENCES MoodTags(TagId) ON DELETE CASCADE
                    )";
                ExecuteCommand(createRecordTagTable, connection);

                // 创建好友关系表
                string createFriendTable = @"
                    CREATE TABLE IF NOT EXISTS Friends (
                        UserId INTEGER NOT NULL,
                        FriendId INTEGER NOT NULL,
                        Status INTEGER DEFAULT 0,
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                        PRIMARY KEY (UserId, FriendId),
                        FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
                        FOREIGN KEY (FriendId) REFERENCES Users(UserId) ON DELETE CASCADE
                    )";
                ExecuteCommand(createFriendTable, connection);

                // 创建点赞表
                string createLikeTable = @"
                    CREATE TABLE IF NOT EXISTS Likes (
                        LikeId INTEGER PRIMARY KEY AUTOINCREMENT,
                        RecordId INTEGER NOT NULL,
                        UserId INTEGER NOT NULL,
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (RecordId) REFERENCES MoodRecords(RecordId) ON DELETE CASCADE,
                        FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
                    )";
                ExecuteCommand(createLikeTable, connection);

                // 创建评论表
                string createCommentTable = @"
                    CREATE TABLE IF NOT EXISTS Comments (
                        CommentId INTEGER PRIMARY KEY AUTOINCREMENT,
                        RecordId INTEGER NOT NULL,
                        UserId INTEGER NOT NULL,
                        CommentText TEXT NOT NULL,
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (RecordId) REFERENCES MoodRecords(RecordId) ON DELETE CASCADE,
                        FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
                    )";
                ExecuteCommand(createCommentTable, connection);

                // 创建表情包表
                string createEmojiTable = @"
                    CREATE TABLE IF NOT EXISTS Emojis (
                        EmojiId INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER DEFAULT 0,
                        EmojiName TEXT NOT NULL,
                        EmojiPath TEXT,
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
                    )";
                ExecuteCommand(createEmojiTable, connection);

                // 创建记录表情关联表
                string createRecordEmojiTable = @"
                    CREATE TABLE IF NOT EXISTS RecordEmojis (
                        RecordId INTEGER NOT NULL,
                        EmojiId INTEGER NOT NULL,
                        PRIMARY KEY (RecordId, EmojiId),
                        FOREIGN KEY (RecordId) REFERENCES MoodRecords(RecordId) ON DELETE CASCADE,
                        FOREIGN KEY (EmojiId) REFERENCES Emojis(EmojiId) ON DELETE CASCADE
                    )";
                ExecuteCommand(createRecordEmojiTable, connection);
            }
        }

        private static void UpdateTableStructure(SQLiteConnection connection)
        {
            try
            {
                // 检查Users表是否已有DefaultPrivacyMode列
                string checkColumnQuery = "PRAGMA table_info(Users)";
                using (var command = new SQLiteCommand(checkColumnQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        bool hasDefaultPrivacyMode = false;
                        while (reader.Read())
                        {
                            if (reader.GetString(1) == "DefaultPrivacyMode")
                            {
                                hasDefaultPrivacyMode = true;
                                break;
                            }
                        }
                        
                        // 如果没有DefaultPrivacyMode列，则添加
                        if (!hasDefaultPrivacyMode)
                        {
                            string addColumnQuery = "ALTER TABLE Users ADD COLUMN DefaultPrivacyMode INTEGER DEFAULT 1";
                            ExecuteCommand(addColumnQuery, connection);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 忽略错误，可能是因为列已存在
            }
        }

        private static void ExecuteCommand(string commandText, SQLiteConnection connection)
        {
            using (var command = new SQLiteCommand(commandText, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public static string GetConnectionString()
        {
            return connectionString;
        }
    }
}