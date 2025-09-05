using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using MoodDiaryApp.Models;

namespace MoodDiaryApp.Data
{
    public class LikeDAO
    {
        public static List<Like> GetLikesByRecordId(int recordId)
        {
            var likes = new List<Like>();
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Likes WHERE RecordId = @RecordId ORDER BY CreatedAt DESC";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RecordId", recordId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            likes.Add(new Like
                            {
                                LikeId = reader.GetInt32(0),
                                RecordId = reader.GetInt32(1),
                                UserId = reader.GetInt32(2),
                                CreatedAt = reader.GetDateTime(3)
                            });
                        }
                    }
                }
            }
            return likes;
        }

        public static List<Like> GetLikesByUserId(int userId)
        {
            var likes = new List<Like>();
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Likes WHERE UserId = @UserId ORDER BY CreatedAt DESC";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            likes.Add(new Like
                            {
                                LikeId = reader.GetInt32(0),
                                RecordId = reader.GetInt32(1),
                                UserId = reader.GetInt32(2),
                                CreatedAt = reader.GetDateTime(3)
                            });
                        }
                    }
                }
            }
            return likes;
        }

        public static bool AddLike(int recordId, int userId)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                // 检查是否已经点赞
                string checkQuery = "SELECT COUNT(*) FROM Likes WHERE RecordId = @RecordId AND UserId = @UserId";
                using (var checkCommand = new SQLiteCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@RecordId", recordId);
                    checkCommand.Parameters.AddWithValue("@UserId", userId);
                    long count = (long)checkCommand.ExecuteScalar();
                    if (count > 0)
                    {
                        return false; // 已经点赞
                    }
                }

                // 添加点赞
                string query = "INSERT INTO Likes (RecordId, UserId, CreatedAt) VALUES (@RecordId, @UserId, @CreatedAt)";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RecordId", recordId);
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static bool RemoveLike(int recordId, int userId)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                // 删除点赞
                string query = "DELETE FROM Likes WHERE RecordId = @RecordId AND UserId = @UserId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RecordId", recordId);
                    command.Parameters.AddWithValue("@UserId", userId);
                    
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static int GetLikeCountByRecordId(int recordId)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Likes WHERE RecordId = @RecordId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RecordId", recordId);
                    long count = (long)command.ExecuteScalar();
                    return (int)count;
                }
            }
        }

        public static bool HasUserLikedRecord(int recordId, int userId)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                // 检查用户是否已点赞
                string query = "SELECT COUNT(*) FROM Likes WHERE RecordId = @RecordId AND UserId = @UserId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RecordId", recordId);
                    command.Parameters.AddWithValue("@UserId", userId);
                    long count = (long)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
    }
}