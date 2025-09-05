using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodDiaryApp.Models
{
    public class Friend
    {
        public int UserId { get; set; }
        public int FriendId { get; set; }
        public int Status { get; set; } // 0-待确认，1-已确认
        public DateTime CreatedAt { get; set; }

        public Friend()
        {
            CreatedAt = DateTime.Now;
        }
    }
}