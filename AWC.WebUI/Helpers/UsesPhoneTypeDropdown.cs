using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AWC.Domain;

namespace AWC.WebUI.Helpers
{
    public class UsesPhoneTypeDropdown : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var viewResult = filterContext.Result as ViewResult;
            if (viewResult != null)
            {
                var values = Enum.GetValues(typeof(Constants.PhoneNumberTypeId)).Cast<Constants.PhoneNumberTypeId>().ToList();
                var phoneTypes = new List<object>();
                for (int i = 0; i < values.Count; i++)
                {
                    phoneTypes.Add(new { Name = values[i], Value = i + 1});
                }
                viewResult.ViewData["PhoneTypes"] = phoneTypes;
            }
        }
    }
}