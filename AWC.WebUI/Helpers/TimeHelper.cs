using System;
using System.Web.Mvc;

namespace AWC.WebUI.Helpers
{
    public static class TimeHelper
    {
        public static MvcHtmlString DateTimeDisplay(this HtmlHelper helper, DateTime gmtDateTime)
        {
            DateTime dateTime = gmtDateTime.ToLocalTime();
            return new MvcHtmlString(dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString());
        }
    }
}