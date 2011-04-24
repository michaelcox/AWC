using System;
using System.Web.Mvc;

namespace AWC.WebUI.Helpers
{
    public static class TimeHelper
    {
        public static MvcHtmlString DateTimeDisplay(this HtmlHelper helper, byte[] timestamp)
        {
            long time = BitConverter.ToInt64(timestamp, 0);
            DateTime dateTime = DateTime.FromBinary(time);
            return new MvcHtmlString(dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString());
        }
    }
}