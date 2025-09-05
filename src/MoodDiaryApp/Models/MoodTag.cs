using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodDiaryApp.Models
{
    public class MoodTag
    {
        public int TagId { get; set; }
        public string TagName { get; set; } = string.Empty;
        public string? TagCategory { get; set; }
        public string? Color { get; set; }

        public MoodTag()
        {
        }
    }
}