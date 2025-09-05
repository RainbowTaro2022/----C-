using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using Newtonsoft.Json;
using MoodDiaryApp.Models;
using MoodDiaryApp.Data;

namespace MoodDiaryApp.Services
{
    /// <summary>
    /// 云同步数据模型
    /// </summary>
    public class CloudSyncData
    {
        public List<User> Users { get; set; }
        public List<MoodRecord> MoodRecords { get; set; }
        public List<MoodTag> MoodTags { get; set; }
        public List<RecordTag> RecordTags { get; set; }
        public List<Friend> Friends { get; set; }
        public List<Like> Likes { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Emoji> Emojis { get; set; }
        public List<RecordEmoji> RecordEmojis { get; set; }
        public DateTime SyncTime { get; set; }
    }

    /// <summary>
    /// 云同步服务
    /// </summary>
    public class CloudSyncService
    {
        private string cloudDirectory;
        
        public CloudSyncService()
        {
            // 创建云同步目录
            cloudDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MoodDiary", "Cloud");
            if (!Directory.Exists(cloudDirectory))
            {
                Directory.CreateDirectory(cloudDirectory);
            }
        }
        
        /// <summary>
        /// 导出数据到云
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>是否成功</returns>
        public bool ExportToCloud(int userId)
        {
            try
            {
                // 获取用户数据
                var user = UserDAO.GetUserById(userId);
                if (user == null)
                    return false;
                
                // 获取用户相关数据
                var moodRecords = MoodRecordDAO.GetMoodRecordsByUserId(userId);
                var userFriends = FriendDAO.GetFriendsByUserId(userId);
                
                // 将User列表转换为Friend列表
                var friends = userFriends.Select(f => new Friend
                {
                    UserId = userId,
                    FriendId = f.UserId,
                    Status = 1, // 已确认的好友关系
                    CreatedAt = DateTime.Now
                }).ToList();
                
                var friendIds = userFriends.Select(f => f.UserId).ToList();
                friendIds.Add(userId); // 包含用户自己
                
                // 获取相关数据
                var likes = new List<Like>();
                var comments = new List<Comment>();
                var emojis = new List<Emoji>();
                var recordEmojis = new List<RecordEmoji>();
                
                foreach (var record in moodRecords)
                {
                    likes.AddRange(LikeDAO.GetLikesByRecordId(record.RecordId));
                    comments.AddRange(CommentDAO.GetCommentsByRecordId(record.RecordId));
                }
                
                // 获取用户表情包
                emojis.AddRange(EmojiDAO.GetUserEmojis(userId));
                
                // 获取记录表情关联
                foreach (var record in moodRecords)
                {
                    var emojisForRecord = EmojiDAO.GetEmojisByRecordId(record.RecordId);
                    recordEmojis.AddRange(emojisForRecord.Select(e => new RecordEmoji
                    {
                        RecordId = record.RecordId,
                        EmojiId = e.EmojiId
                    }));
                }
                
                // 创建云同步数据对象
                var syncData = new CloudSyncData
                {
                    Users = new List<User> { user },
                    MoodRecords = moodRecords,
                    MoodTags = new List<MoodTag>(), // 标签是全局的，可以单独处理
                    RecordTags = new List<RecordTag>(), // 标签关联也是全局的
                    Friends = friends,
                    Likes = likes,
                    Comments = comments,
                    Emojis = emojis,
                    RecordEmojis = recordEmojis,
                    SyncTime = DateTime.Now
                };
                
                // 序列化为JSON
                string json = JsonConvert.SerializeObject(syncData, Formatting.Indented);
                
                // 保存到云同步目录
                string filePath = Path.Combine(cloudDirectory, $"mooddiary_sync_{userId}_{DateTime.Now:yyyyMMddHHmmss}.json");
                File.WriteAllText(filePath, json, Encoding.UTF8);
                
                return true;
            }
            catch (Exception ex)
            {
                // 记录错误日志
                System.Diagnostics.Debug.WriteLine($"导出到云失败: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// 从云导入数据
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="filePath">云同步文件路径</param>
        /// <returns>是否成功</returns>
        public bool ImportFromCloud(int userId, string filePath)
        {
            try
            {
                // 读取云同步文件
                if (!File.Exists(filePath))
                    return false;
                
                string json = File.ReadAllText(filePath, Encoding.UTF8);
                var syncData = JsonConvert.DeserializeObject<CloudSyncData>(json);
                
                if (syncData == null)
                    return false;
                
                // 开始事务处理
                using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 导入心情记录
                            foreach (var record in syncData.MoodRecords)
                            {
                                // 检查记录是否已存在
                                var existingRecord = MoodRecordDAO.GetMoodRecordById(record.RecordId);
                                if (existingRecord == null)
                                {
                                    // 插入新记录
                                    MoodRecordDAO.AddMoodRecord(record);
                                }
                                else
                                {
                                    // 更新现有记录
                                    MoodRecordDAO.UpdateMoodRecord(record);
                                }
                            }
                            
                            // 导入好友关系
                            foreach (var friend in syncData.Friends)
                            {
                                // 检查好友关系是否已存在
                                if (!FriendDAO.AreFriends(friend.UserId, friend.FriendId))
                                {
                                    FriendDAO.AddFriend(friend.UserId, friend.FriendId);
                                }
                            }
                            
                            // 导入点赞
                            foreach (var like in syncData.Likes)
                            {
                                // 检查点赞是否已存在
                                if (!LikeDAO.HasUserLikedRecord(like.RecordId, like.UserId))
                                {
                                    LikeDAO.AddLike(like.RecordId, like.UserId);
                                }
                            }
                            
                            // 导入评论
                            foreach (var comment in syncData.Comments)
                            {
                                int commentId = CommentDAO.AddComment(comment);
                                // 可以根据需要处理返回的commentId
                            }
                            
                            // 导入表情包
                            foreach (var emoji in syncData.Emojis)
                            {
                                // 检查表情包是否已存在
                                var existingEmoji = EmojiDAO.GetEmojiById(emoji.EmojiId);
                                if (existingEmoji == null)
                                {
                                    EmojiDAO.AddEmoji(emoji);
                                }
                            }
                            
                            // 导入记录表情关联
                            foreach (var recordEmoji in syncData.RecordEmojis)
                            {
                                EmojiDAO.AddEmojiToRecord(recordEmoji.RecordId, recordEmoji.EmojiId);
                            }
                            
                            // 提交事务
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            // 回滚事务
                            transaction.Rollback();
                            System.Diagnostics.Debug.WriteLine($"从云导入失败: {ex.Message}");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"从云导入失败: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// 获取云同步目录中的所有同步文件
        /// </summary>
        /// <returns>同步文件列表</returns>
        public List<string> GetCloudSyncFiles()
        {
            try
            {
                var files = Directory.GetFiles(cloudDirectory, "mooddiary_sync_*.json")
                    .OrderByDescending(f => f)
                    .ToList();
                return files;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"获取云同步文件失败: {ex.Message}");
                return new List<string>();
            }
        }
        
        /// <summary>
        /// 删除云同步文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>是否成功</returns>
        public bool DeleteCloudSyncFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"删除云同步文件失败: {ex.Message}");
                return false;
            }
        }
    }
}