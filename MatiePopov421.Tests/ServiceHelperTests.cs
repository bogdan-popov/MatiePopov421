using System;
using Xunit;

namespace MatiePopov421.Tests
{
    public static class ServiceHelper
    {
        public static string FormatLastModified(DateTime dateTime)
        {
            return dateTime.ToString("dd.MM.yyyy HH:mm");
        }
    }

    public class ServiceHelperTests
    {
        [Fact]
        public void FormatLastModified_ReturnsCorrectFormat()
        {
            var dateTime = new DateTime(2025, 6, 15, 14, 30, 0);
            var result = ServiceHelper.FormatLastModified(dateTime);
            Assert.Equal("15.06.2025 14:30", result);
        }

        [Fact]
        public void FormatLastModified_MidnightReturnsCorrectTime()
        {
            var dateTime = new DateTime(2024, 1, 1, 0, 0, 0);
            var result = ServiceHelper.FormatLastModified(dateTime);
            Assert.Equal("01.01.2024 00:00", result);
        }

        [Fact]
        public void FormatLastModified_NowIsNotEmpty()
        {
            var result = ServiceHelper.FormatLastModified(DateTime.Now);
            Assert.False(string.IsNullOrEmpty(result));
        }
    }
}
