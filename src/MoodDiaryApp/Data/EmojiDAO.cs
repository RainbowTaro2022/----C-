using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using MoodDiaryApp.Models;
using System.IO;

namespace MoodDiaryApp.Data
{
    public class EmojiDAO
    {
        public static Emoji GetEmojiById(int emojiId)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Emojis WHERE EmojiId = @EmojiId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmojiId", emojiId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Emoji
                            {
                                EmojiId = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                EmojiName = reader.GetString(2),
                                EmojiPath = reader.IsDBNull(3) ? null : reader.GetString(3),
                                CreatedAt = reader.GetDateTime(4)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public static List<Emoji> GetEmojisByUserId(int userId)
        {
            var emojis = new List<Emoji>();
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Emojis WHERE UserId = @UserId OR UserId = 0 ORDER BY CreatedAt DESC";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            emojis.Add(new Emoji
                            {
                                EmojiId = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                EmojiName = reader.GetString(2),
                                EmojiPath = reader.IsDBNull(3) ? null : reader.GetString(3),
                                CreatedAt = reader.GetDateTime(4)
                            });
                        }
                    }
                }
            }
            return emojis;
        }

        public static List<Emoji> GetSystemEmojis()
        {
            var emojis = new List<Emoji>();
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Emojis WHERE UserId = 0 ORDER BY CreatedAt DESC";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            emojis.Add(new Emoji
                            {
                                EmojiId = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                EmojiName = reader.GetString(2),
                                EmojiPath = reader.IsDBNull(3) ? null : reader.GetString(3),
                                CreatedAt = reader.GetDateTime(4)
                            });
                        }
                    }
                }
            }
            return emojis;
        }

        public static List<Emoji> GetUserEmojis(int userId)
        {
            var emojis = new List<Emoji>();
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Emojis WHERE UserId = @UserId ORDER BY CreatedAt DESC";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            emojis.Add(new Emoji
                            {
                                EmojiId = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                EmojiName = reader.GetString(2),
                                EmojiPath = reader.IsDBNull(3) ? null : reader.GetString(3),
                                CreatedAt = reader.GetDateTime(4)
                            });
                        }
                    }
                }
            }
            return emojis;
        }

        public static int AddEmoji(Emoji emoji)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = @"INSERT INTO Emojis (UserId, EmojiName, EmojiPath, CreatedAt) 
                                VALUES (@UserId, @EmojiName, @EmojiPath, @CreatedAt); 
                                SELECT last_insert_rowid();";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", emoji.UserId);
                    command.Parameters.AddWithValue("@EmojiName", emoji.EmojiName);
                    command.Parameters.AddWithValue("@EmojiPath", emoji.EmojiPath ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedAt", emoji.CreatedAt);
                    
                    long id = (long)command.ExecuteScalar();
                    return (int)id;
                }
            }
        }

        public static bool UpdateEmoji(Emoji emoji)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = @"UPDATE Emojis SET EmojiName = @EmojiName, EmojiPath = @EmojiPath 
                                WHERE EmojiId = @EmojiId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmojiId", emoji.EmojiId);
                    command.Parameters.AddWithValue("@EmojiName", emoji.EmojiName);
                    command.Parameters.AddWithValue("@EmojiPath", emoji.EmojiPath ?? (object)DBNull.Value);
                    
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static bool DeleteEmoji(int emojiId)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "DELETE FROM Emojis WHERE EmojiId = @EmojiId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmojiId", emojiId);
                    
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static List<Emoji> GetEmojisByRecordId(int recordId)
        {
            var emojis = new List<Emoji>();
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = @"SELECT e.* FROM Emojis e 
                                INNER JOIN RecordEmojis re ON e.EmojiId = re.EmojiId 
                                WHERE re.RecordId = @RecordId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RecordId", recordId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            emojis.Add(new Emoji
                            {
                                EmojiId = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                EmojiName = reader.GetString(2),
                                EmojiPath = reader.IsDBNull(3) ? null : reader.GetString(3),
                                CreatedAt = reader.GetDateTime(4)
                            });
                        }
                    }
                }
            }
            return emojis;
        }

        public static bool AddEmojiToRecord(int recordId, int emojiId)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = @"INSERT OR IGNORE INTO RecordEmojis (RecordId, EmojiId) 
                                VALUES (@RecordId, @EmojiId)";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RecordId", recordId);
                    command.Parameters.AddWithValue("@EmojiId", emojiId);
                    
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static bool RemoveEmojiFromRecord(int recordId, int emojiId)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "DELETE FROM RecordEmojis WHERE RecordId = @RecordId AND EmojiId = @EmojiId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RecordId", recordId);
                    command.Parameters.AddWithValue("@EmojiId", emojiId);
                    
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}