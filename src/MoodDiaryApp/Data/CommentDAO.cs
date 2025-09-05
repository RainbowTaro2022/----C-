using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using MoodDiaryApp.Models;

namespace MoodDiaryApp.Data
{
    public class CommentDAO
    {
        public static List<Comment> GetCommentsByRecordId(int recordId)
        {
            var comments = new List<Comment>();
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Comments WHERE RecordId = @RecordId ORDER BY CreatedAt DESC";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RecordId", recordId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comments.Add(new Comment
                            {
                                CommentId = reader.GetInt32(0),
                                RecordId = reader.GetInt32(1),
                                UserId = reader.GetInt32(2),
                                CommentText = reader.GetString(3),
                                CreatedAt = reader.GetDateTime(4)
                            });
                        }
                    }
                }
            }
            return comments;
        }

        public static List<Comment> GetCommentsByUserId(int userId)
        {
            var comments = new List<Comment>();
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Comments WHERE UserId = @UserId ORDER BY CreatedAt DESC";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comments.Add(new Comment
                            {
                                CommentId = reader.GetInt32(0),
                                RecordId = reader.GetInt32(1),
                                UserId = reader.GetInt32(2),
                                CommentText = reader.GetString(3),
                                CreatedAt = reader.GetDateTime(4)
                            });
                        }
                    }
                }
            }
            return comments;
        }

        public static int AddComment(Comment comment)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = @"INSERT INTO Comments (RecordId, UserId, CommentText, CreatedAt) 
                                VALUES (@RecordId, @UserId, @CommentText, @CreatedAt);
                                SELECT last_insert_rowid();";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RecordId", comment.RecordId);
                    command.Parameters.AddWithValue("@UserId", comment.UserId);
                    command.Parameters.AddWithValue("@CommentText", comment.CommentText);
                    command.Parameters.AddWithValue("@CreatedAt", comment.CreatedAt);
                    
                    long id = (long)command.ExecuteScalar();
                    return (int)id;
                }
            }
        }

        public static bool UpdateComment(Comment comment)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "UPDATE Comments SET CommentText = @CommentText WHERE CommentId = @CommentId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CommentId", comment.CommentId);
                    command.Parameters.AddWithValue("@CommentText", comment.CommentText);
                    
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static bool DeleteComment(int commentId)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "DELETE FROM Comments WHERE CommentId = @CommentId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CommentId", commentId);
                    
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static int GetCommentCountByRecordId(int recordId)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Comments WHERE RecordId = @RecordId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RecordId", recordId);
                    long count = (long)command.ExecuteScalar();
                    return (int)count;
                }
            }
        }
    }
}