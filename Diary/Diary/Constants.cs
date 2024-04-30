namespace Diary;

public static class Constants
{
    public static string AppFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

    public static string DatabasePath => Path.Combine(AppFolder, "DiaryAppDb.db3");

    public const string FirstRunKey = "FirstRun";

    public const string DefaultImportExportFileName = "DiaryData.json";

    public const string MapDateTimeFormat = "dd/MM/y H:mm";

    public const string BingMapsApiKey = "AslgOYkoOW5Q3lhZ0c3az5GzKnFtvrWQjayd4ihqT5nNOYr6WUzoXLX6o4yHAwE9";

    public static TimeSpan CancellationTokenDelay = TimeSpan.FromSeconds(5);
}
