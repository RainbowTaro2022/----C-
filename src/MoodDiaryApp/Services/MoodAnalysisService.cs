using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoodDiaryApp.Models;

namespace MoodDiaryApp.Services
{
    public class MoodAnalysisService
    {
        // 简单的情感词典
        private static readonly Dictionary<string, int> EmotionDictionary = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            // 积极情绪
            {"开心", 2}, {"快乐", 2}, {"高兴", 2}, {"愉悦", 2}, {"兴奋", 2}, {"满足", 2}, {"幸福", 2}, {"乐观", 2},
            {"喜悦", 2}, {"欢欣", 2}, {"欣喜", 2}, {"愉快", 2}, {"舒畅", 2}, {"轻松", 1}, {"安心", 1}, {"平静", 1},
            
            // 消极情绪
            {"悲伤", -2}, {"难过", -2}, {"沮丧", -2}, {"失望", -2}, {"绝望", -3}, {"痛苦", -3}, {"伤心", -2},
            {"郁闷", -2}, {"烦躁", -2}, {"焦虑", -2}, {"担忧", -2}, {"害怕", -2}, {"恐惧", -3}, {"愤怒", -3},
            {"生气", -3}, {"恼怒", -3}, {"厌恶", -2}, {"嫉妒", -2}, {"孤独", -2}, {"寂寞", -2}, {"疲惫", -1},
            
            // 中性情绪
            {"无聊", 0}, {"困倦", 0}, {"麻木", 0}, {"茫然", 0}, {"犹豫", 0}
        };

        /// <summary>
        /// 分析心情文本并返回情绪评分
        /// </summary>
        /// <param name="moodText">心情文本</param>
        /// <returns>情绪评分 (-3到3)</returns>
        public static int AnalyzeMoodScore(string moodText)
        {
            if (string.IsNullOrWhiteSpace(moodText))
                return 0;

            int totalScore = 0;
            int wordCount = 0;

            // 简单的分词处理
            foreach (var kvp in EmotionDictionary)
            {
                if (moodText.Contains(kvp.Key))
                {
                    totalScore += kvp.Value;
                    wordCount++;
                }
            }

            // 如果没有匹配到任何情绪词，默认为中性
            if (wordCount == 0)
                return 0;

            // 计算平均分并限制在-3到3之间
            int averageScore = totalScore / wordCount;
            return Math.Max(-3, Math.Min(3, averageScore));
        }

        /// <summary>
        /// 获取情绪标签建议
        /// </summary>
        /// <param name="moodText">心情文本</param>
        /// <returns>建议的情绪标签列表</returns>
        public static List<string> GetSuggestedTags(string moodText)
        {
            var suggestedTags = new List<string>();

            if (string.IsNullOrWhiteSpace(moodText))
                return suggestedTags;

            foreach (var kvp in EmotionDictionary)
            {
                if (moodText.Contains(kvp.Key) && !suggestedTags.Contains(kvp.Key))
                {
                    suggestedTags.Add(kvp.Key);
                }
            }

            return suggestedTags;
        }

        /// <summary>
        /// 生成情绪变化趋势数据
        /// </summary>
        /// <param name="records">心情记录列表</param>
        /// <returns>情绪趋势数据点</returns>
        public static List<KeyValuePair<DateTime, int>> GenerateMoodTrendData(List<MoodRecord> records)
        {
            var trendData = new List<KeyValuePair<DateTime, int>>();

            // 按日期分组并计算平均情绪分
            var groupedRecords = records
                .GroupBy(r => r.RecordDate.Date)
                .Select(g => new { Date = g.Key, AverageScore = (int)g.Average(r => r.MoodScore) })
                .OrderBy(g => g.Date);

            foreach (var group in groupedRecords)
            {
                trendData.Add(new KeyValuePair<DateTime, int>(group.Date, group.AverageScore));
            }

            return trendData;
        }
    }
}