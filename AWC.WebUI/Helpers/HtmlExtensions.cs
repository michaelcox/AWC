using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using AWC.Domain;

namespace AWC.WebUI.Helpers
{
    public static class HtmlExtensions
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

        public static MvcHtmlString ActionMenuItem(this HtmlHelper htmlHelper, String linkText, String actionName, String controllerName, bool matchControllerOnly = false, object routeValues = null)
        {
            var tag = new TagBuilder("li");

            if (htmlHelper.ViewContext.RequestContext.IsCurrentRoute(null, controllerName, matchControllerOnly, actionName))
            {
                tag.AddCssClass("active");
            }

            tag.InnerHtml = htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, null).ToString();

            return MvcHtmlString.Create(tag.ToString());
        }

        private static bool IsCurrentRoute(this RequestContext context, String areaName, String controllerName, bool matchControllerOnly, params String[] actionNames)
        {
            var routeData = context.RouteData;
            var routeArea = routeData.DataTokens["area"] as String;
            var routeController = routeData.GetRequiredString("controller");
            var routeAction = routeData.GetRequiredString("action");
            
            // Check the area - return false if we're not in the right area
            if (!string.IsNullOrEmpty(areaName) && !string.IsNullOrEmpty(routeArea) && !routeArea.IsSameAs(areaName,  StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            // Check the controller
            if (!string.IsNullOrEmpty(controllerName) && !string.IsNullOrEmpty(routeController) && !routeController.IsSameAs(controllerName, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (matchControllerOnly)
            {
                return true;
            }

            if (actionNames == null)
            {
                return true;
            }

            if (actionNames.Contains(routeAction, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private static bool IsSameAs(this string original, string value, StringComparison comparisonType)
        {
            return original.IndexOf(value, comparisonType) >= 0;
        }

        private static bool Contains(this IEnumerable<string> list, string value, StringComparison comparisonType)
        {
            foreach (string s in list)
            {
                if (s.IsSameAs(value, comparisonType))
                {
                    return true;
                }
            }
            return false;
        }
    }

}