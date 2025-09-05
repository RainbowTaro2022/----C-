using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using MoodDiaryApp.Models;

namespace MoodDiaryApp.Data
{
    public class FriendDAO
    {
        public static List<User> GetFriendsByUserId(int userId)
        {
            var friends = new List<User>();
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                // 查询已确认的好友关系
                string query = @"
                    SELECT u.* FROM Users u 
                    INNER JOIN Friends f ON (u.UserId = f.FriendId OR u.UserId = f.UserId)
                    WHERE (f.UserId = @UserId OR f.FriendId = @UserId) 
                    AND f.Status = 1 
                    AND u.UserId != @UserId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            friends.Add(new User
                            {
                                UserId = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Password = reader.GetString(2),
                                Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                                CreatedAt = reader.GetDateTime(4),
                                LastLogin = reader.GetDateTime(5)
                            });
                        }
                    }
                }
            }
            return friends;
        }

        public static List<User> GetPendingFriendsByUserId(int userId)
        {
            var pendingFriends = new List<User>();
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                // 查询待确认的好友请求
                string query = @"
                    SELECT u.* FROM Users u 
                    INNER JOIN Friends f ON u.UserId = f.UserId
                    WHERE f.FriendId = @UserId AND f.Status = 0";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pendingFriends.Add(new User
                            {
                                UserId = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Password = reader.GetString(2),
                                Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                                CreatedAt = reader.GetDateTime(4),
                                LastLogin = reader.GetDateTime(5)
                            });
                        }
                    }
                }
            }
            return pendingFriends;
        }

        public static bool AddFriend(int userId, int friendId)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                // 检查是否已经存在好友关系
                string checkQuery = "SELECT COUNT(*) FROM Friends WHERE (UserId = @UserId AND FriendId = @FriendId) OR (UserId = @FriendId AND FriendId = @UserId)";
                using (var checkCommand = new SQLiteCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@UserId", userId);
                    checkCommand.Parameters.AddWithValue("@FriendId", friendId);
                    long count = (long)checkCommand.ExecuteScalar();
                    if (count > 0)
                    {
                        return false; // 好友关系已存在
                    }
                }

                // 添加好友请求
                string query = "INSERT INTO Friends (UserId, FriendId, Status, CreatedAt) VALUES (@UserId, @FriendId, 0, @CreatedAt)";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@FriendId", friendId);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static bool ConfirmFriend(int userId, int friendId)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                // 确认好友请求
                string query = "UPDATE Friends SET Status = 1 WHERE UserId = @FriendId AND FriendId = @UserId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@FriendId", friendId);
                    
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static bool DeleteFriend(int userId, int friendId)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                // 删除好友关系
                string query = "DELETE FROM Friends WHERE (UserId = @UserId AND FriendId = @FriendId) OR (UserId = @FriendId AND FriendId = @UserId)";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@FriendId", friendId);
                    
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static bool AreFriends(int userId, int friendId)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                // 检查好友关系是否存在且已确认
                string query = "SELECT COUNT(*) FROM Friends WHERE ((UserId = @UserId AND FriendId = @FriendId) OR (UserId = @FriendId AND FriendId = @UserId)) AND Status = 1";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@FriendId", friendId);
                    long count = (long)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
    }
}