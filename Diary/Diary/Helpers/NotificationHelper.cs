namespace Diary.Helpers
{
    public static class NotificationHelper
    {
        public static int GetNotificationIdFromCreationDate(DateTime creationDate)
        {
            return creationDate.Month * 100 + creationDate.Day;
        }
    }
}
