namespace Diary;

public static class Constants
{
    public static string DatabasePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Personal),
        "DiaryAppDb.db3"
        );

    public const string FirstRunKey = "FirstRun";
}
