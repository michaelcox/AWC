using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AWC.Domain;

namespace AWC.WebUI.Helpers
{
    public static class StringHelper
    {
        public static MvcHtmlString PhoneDisplay(this HtmlHelper helper, string phoneNumber)
        {
            if (!string.IsNullOrEmpty(phoneNumber) && phoneNumber.Length == 10)
            {
                return new MvcHtmlString(string.Format("{0}-{1}-{2}", phoneNumber.Substring(0, 3), phoneNumber.Substring(3, 3), phoneNumber.Substring(7)));
            }
            if (!string.IsNullOrEmpty(phoneNumber) && phoneNumber.Length == 7)
            {
                return new MvcHtmlString(string.Format("{0}-{1}", phoneNumber.Substring(0, 3), phoneNumber.Substring(3)));
            }

            return new MvcHtmlString(string.Empty);
        }

        public static MvcHtmlString DateDisplay(this HtmlHelper helper, DateTime date)
        {
            return new MvcHtmlString(date.ToString("MM/dd/yyyy"));
        }

        public static MvcHtmlString DateTimeDisplay(this HtmlHelper helper, DateTime gmtDateTime)
        {
            DateTime dateTime = gmtDateTime.ToLocalTime();
            return new MvcHtmlString(dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString());
        }

        public static MvcHtmlString PhoneType(this HtmlHelper helper, byte phoneNumberTypeId)
        {
            return new MvcHtmlString(Enum.Parse(typeof (Constants.PhoneNumberTypeId), phoneNumberTypeId.ToString()).ToString());
        }
    }
}