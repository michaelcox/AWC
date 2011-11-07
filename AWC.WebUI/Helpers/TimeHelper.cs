using System;
using System.Configuration;

namespace AWC.WebUI.Helpers
{
    public static class TimeHelper
    {
        public static DateTime ConvertToLocal(DateTime utcTime)
        {
            var localZone = TimeZoneInfo.FindSystemTimeZoneById(ConfigurationManager.AppSettings["TimezoneId"]);
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, localZone);
        }
    }
}