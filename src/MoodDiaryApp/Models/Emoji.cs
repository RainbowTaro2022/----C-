using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodDiaryApp.Models
{
    public class Emoji
    {
        public int EmojiId { get; set; }
        public int UserId { get; set; } // 系统表情包为0
        public string EmojiName { get; set; } = string.Empty;
        public string? EmojiPath { get; set; }
        public DateTime CreatedAt { get; set; }

        public Emoji()
        {
            CreatedAt = DateTime.Now;
        }
    }
}