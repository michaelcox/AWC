using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWC.WebUI.Helpers
{
    public static class Constants
    {
        public enum AppointmentStatusId
        {
            NotScheduled = 1,
            Scheduled = 2,
            Rescheduled = 3,
            Closed = 4
        }
    }
}