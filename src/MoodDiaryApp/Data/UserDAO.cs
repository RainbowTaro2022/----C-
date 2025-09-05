using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using MoodDiaryApp.Models;

namespace MoodDiaryApp.Data
{
    public class UserDAO
    {
        public static User GetUserById(int userId)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Users WHERE UserId = @UserId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserId = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Password = reader.GetString(2),
                                Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                                DefaultPrivacyMode = reader.IsDBNull(4) ? 1 : reader.GetInt32(4),
                                CreatedAt = reader.GetDateTime(5),
                                LastLogin = reader.GetDateTime(6)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public static User GetUserByUsername(string username)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Users WHERE Username = @Username";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserId = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Password = reader.GetString(2),
                                Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                                DefaultPrivacyMode = reader.IsDBNull(4) ? 1 : reader.GetInt32(4),
                                CreatedAt = reader.GetDateTime(5),
                                LastLogin = reader.GetDateTime(6)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public static int AddUser(User user)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "INSERT INTO Users (Username, Password, Email, DefaultPrivacyMode, CreatedAt, LastLogin) VALUES (@Username, @Password, @Email, @DefaultPrivacyMode, @CreatedAt, @LastLogin); SELECT last_insert_rowid();";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DefaultPrivacyMode", user.DefaultPrivacyMode);
                    command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);
                    command.Parameters.AddWithValue("@LastLogin", user.LastLogin);
                    
                    long id = (long)command.ExecuteScalar();
                    return (int)id;
                }
            }
        }

        public static bool UpdateUser(User user)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "UPDATE Users SET Username = @Username, Password = @Password, Email = @Email, DefaultPrivacyMode = @DefaultPrivacyMode, LastLogin = @LastLogin WHERE UserId = @UserId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", user.UserId);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DefaultPrivacyMode", user.DefaultPrivacyMode);
                    command.Parameters.AddWithValue("@LastLogin", user.LastLogin);
                    
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static bool DeleteUser(int userId)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "DELETE FROM Users WHERE UserId = @UserId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}