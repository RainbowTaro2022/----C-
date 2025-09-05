using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodDiaryApp.Models
{
    public class MoodRecord
    {
        public int RecordId { get; set; }
        public int UserId { get; set; }
        public DateTime RecordDate { get; set; }
        public string? MoodText { get; set; }
        public int MoodScore { get; set; }
        public int PrivacyMode { get; set; } // 0-匿名，1-实名
        public string? Location { get; set; }
        public DateTime CreatedAt { get; set; }

        public MoodRecord()
        {
            RecordDate = DateTime.Now;
            CreatedAt = DateTime.Now;
        }
    }
}