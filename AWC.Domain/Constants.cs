namespace AWC.Domain
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

        public enum PhoneNumberTypeId
        {
            Home = 1,
            Mobile = 2,
            Work = 3,
            Other = 4
        }
    }
}