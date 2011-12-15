using System;
using System.Configuration;

namespace AWC.WebUI.Helpers
{
    public static class TimeHelper
    {
        public static DateTime ConvertToLocal(this DateTime utcTime)
        {
            var localZone = TimeZoneInfo.FindSystemTimeZoneById(ConfigurationManager.AppSettings["TimezoneId"]);
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, localZone);
        }

        public static DateTime ConvertToUtc(this DateTime localTime)
        {
            var localZone = TimeZoneInfo.FindSystemTimeZoneById(ConfigurationManager.AppSettings["TimezoneId"]);
            var timeToConvert = new DateTime(localTime.Year, localTime.Month, localTime.Day, localTime.Hour,
                                             localTime.Minute, localTime.Second, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(timeToConvert, localZone);
        }
    }
}