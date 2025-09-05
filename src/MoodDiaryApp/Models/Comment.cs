using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodDiaryApp.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int RecordId { get; set; }
        public int UserId { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public Comment()
        {
            CreatedAt = DateTime.Now;
        }
    }
}