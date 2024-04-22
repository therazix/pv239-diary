namespace Diary;

public static class Constants
{
    public static string AppFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

    public static string DatabasePath => Path.Combine(AppFolder, "DiaryAppDb.db3");

    public const string FirstRunKey = "FirstRun";

    public const string DefaultImportExportFileName = "DiaryAppExport.json";
}
