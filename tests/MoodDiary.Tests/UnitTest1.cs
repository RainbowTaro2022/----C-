using System;
using System.Collections.Generic;
using Xunit;
using MoodDiaryApp.Services;

namespace MoodDiary.Tests
{
    public class MoodAnalysisServiceTests
    {
        [Fact]
        public void AnalyzeMoodScore_PositiveText_ReturnsPositiveScore()
        {
            // Arrange
            string moodText = "今天我很开心，感觉非常愉快";

            // Act
            int score = MoodAnalysisService.AnalyzeMoodScore(moodText);

            // Assert
            Assert.True(score > 0);
        }

        [Fact]
        public void AnalyzeMoodScore_NegativeText_ReturnsNegativeScore()
        {
            // Arrange
            string moodText = "今天我很悲伤，感觉非常沮丧";

            // Act
            int score = MoodAnalysisService.AnalyzeMoodScore(moodText);

            // Assert
            Assert.True(score < 0);
        }

        [Fact]
        public void AnalyzeMoodScore_NeutralText_ReturnsZeroScore()
        {
            // Arrange
            string moodText = "今天天气不错";

            // Act
            int score = MoodAnalysisService.AnalyzeMoodScore(moodText);

            // Assert
            Assert.Equal(0, score);
        }

        [Fact]
        public void GetSuggestedTags_PositiveText_ReturnsPositiveTags()
        {
            // Arrange
            string moodText = "今天我很开心，感觉非常愉快";

            // Act
            var tags = MoodAnalysisService.GetSuggestedTags(moodText);

            // Assert
            Assert.Contains("开心", tags);
            Assert.Contains("愉快", tags);
        }

        [Fact]
        public void GetSuggestedTags_NegativeText_ReturnsNegativeTags()
        {
            // Arrange
            string moodText = "今天我很悲伤，感觉非常沮丧";

            // Act
            var tags = MoodAnalysisService.GetSuggestedTags(moodText);

            // Assert
            Assert.Contains("悲伤", tags);
            Assert.Contains("沮丧", tags);
        }
    }
}