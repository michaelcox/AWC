namespace System.Web.Mvc
{
    public static class FlashHelper
    {
        public static void FlashInfo(this Controller controller, string message)
        {
            controller.TempData["info"] = message;
        }
        public static void FlashSuccess(this Controller controller, string message)
        {
            controller.TempData["success"] = message;
        }
        public static void FlashError(this Controller controller, string message)
        {
            controller.TempData["error"] = message;
        }
    }
}