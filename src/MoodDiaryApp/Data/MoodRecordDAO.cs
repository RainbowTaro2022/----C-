using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using MoodDiaryApp.Models;

namespace MoodDiaryApp.Data
{
    public class MoodRecordDAO
    {
        public static MoodRecord GetMoodRecordById(int recordId)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM MoodRecords WHERE RecordId = @RecordId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RecordId", recordId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new MoodRecord
                            {
                                RecordId = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                RecordDate = reader.GetDateTime(2),
                                MoodText = reader.IsDBNull(3) ? null : reader.GetString(3),
                                MoodScore = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                                PrivacyMode = reader.GetInt32(5),
                                Location = reader.IsDBNull(6) ? null : reader.GetString(6),
                                CreatedAt = reader.GetDateTime(7)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public static List<MoodRecord> GetMoodRecordsByUserId(int userId)
        {
            var records = new List<MoodRecord>();
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM MoodRecords WHERE UserId = @UserId ORDER BY RecordDate DESC";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            records.Add(new MoodRecord
                            {
                                RecordId = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                RecordDate = reader.GetDateTime(2),
                                MoodText = reader.IsDBNull(3) ? null : reader.GetString(3),
                                MoodScore = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                                PrivacyMode = reader.GetInt32(5),
                                Location = reader.IsDBNull(6) ? null : reader.GetString(6),
                                CreatedAt = reader.GetDateTime(7)
                            });
                        }
                    }
                }
            }
            return records;
        }

        public static List<MoodRecord> GetMoodRecordsByDateRange(int userId, DateTime startDate, DateTime endDate)
        {
            var records = new List<MoodRecord>();
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM MoodRecords WHERE UserId = @UserId AND RecordDate BETWEEN @StartDate AND @EndDate ORDER BY RecordDate DESC";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            records.Add(new MoodRecord
                            {
                                RecordId = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                RecordDate = reader.GetDateTime(2),
                                MoodText = reader.IsDBNull(3) ? null : reader.GetString(3),
                                MoodScore = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                                PrivacyMode = reader.GetInt32(5),
                                Location = reader.IsDBNull(6) ? null : reader.GetString(6),
                                CreatedAt = reader.GetDateTime(7)
                            });
                        }
                    }
                }
            }
            return records;
        }

        public static int AddMoodRecord(MoodRecord record)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = @"INSERT INTO MoodRecords (UserId, RecordDate, MoodText, MoodScore, PrivacyMode, Location, CreatedAt) 
                                VALUES (@UserId, @RecordDate, @MoodText, @MoodScore, @PrivacyMode, @Location, @CreatedAt); 
                                SELECT last_insert_rowid();";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", record.UserId);
                    command.Parameters.AddWithValue("@RecordDate", record.RecordDate);
                    command.Parameters.AddWithValue("@MoodText", record.MoodText ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MoodScore", record.MoodScore);
                    command.Parameters.AddWithValue("@PrivacyMode", record.PrivacyMode);
                    command.Parameters.AddWithValue("@Location", record.Location ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedAt", record.CreatedAt);
                    
                    long id = (long)command.ExecuteScalar();
                    return (int)id;
                }
            }
        }

        public static bool UpdateMoodRecord(MoodRecord record)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = @"UPDATE MoodRecords SET RecordDate = @RecordDate, MoodText = @MoodText, MoodScore = @MoodScore, 
                                PrivacyMode = @PrivacyMode, Location = @Location WHERE RecordId = @RecordId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RecordId", record.RecordId);
                    command.Parameters.AddWithValue("@RecordDate", record.RecordDate);
                    command.Parameters.AddWithValue("@MoodText", record.MoodText ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MoodScore", record.MoodScore);
                    command.Parameters.AddWithValue("@PrivacyMode", record.PrivacyMode);
                    command.Parameters.AddWithValue("@Location", record.Location ?? (object)DBNull.Value);
                    
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static bool DeleteMoodRecord(int recordId)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "DELETE FROM MoodRecords WHERE RecordId = @RecordId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RecordId", recordId);
                    
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}