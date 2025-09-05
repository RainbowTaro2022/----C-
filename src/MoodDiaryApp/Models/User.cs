using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodDiaryApp.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
        public int DefaultPrivacyMode { get; set; } // 0-匿名，1-实名

        public User()
        {
            CreatedAt = DateTime.Now;
            LastLogin = DateTime.Now;
        }
    }
}