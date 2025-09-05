using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodDiaryApp.Models
{
    public class Like
    {
        public int LikeId { get; set; }
        public int RecordId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public Like()
        {
            CreatedAt = DateTime.Now;
        }
    }
}