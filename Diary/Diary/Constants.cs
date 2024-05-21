namespace Diary;

public static class Constants
{
    public static readonly string AppFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Diary");
    public static readonly string DatabasePath = Path.Combine(AppFolder, "DiaryDB.db3");
    public static readonly string MediaPath = Path.Combine(AppFolder, "media");
    public static readonly string TempPath = Path.Combine(Path.GetTempPath(), "Diary");
    public static readonly TimeSpan CancellationTokenDelay = TimeSpan.FromSeconds(5);
    public static readonly Color DefaultLabelColor = Color.FromArgb("#FF000000");

    public const string FirstRunKey = "FirstRun";
    public const string DefaultExportFileName = "DiaryExport.zip";
    public const string MapDateTimeFormat = "dd/MM/y H:mm";
    public const string BingMapsApiKey = "AslgOYkoOW5Q3lhZ0c3az5GzKnFtvrWQjayd4ihqT5nNOYr6WUzoXLX6o4yHAwE9";

# if WINDOWS
    public const float LineChartLabelTextSize = 15f;
    public const float LineChartLineSize = 3.5f;

    public const float RadarChartLabelTextSize = 18f;
    public const float RadarChartLineSize = 3.5f;

    public const float PointChartLabelTextSize = 15f;
    public const float PointChartPointSize = 15f;
    public const float PointChartValueLabelTextSize = 20f;

# else
    public const float LineChartLabelTextSize = 40f;
    public const float LineChartLineSize = 7f;

    public const float RadarChartLabelTextSize = 50f;
    public const float RadarChartLineSize = 7f;

    public const float PointChartLabelTextSize = 40f;
    public const float PointChartPointSize = 30f;
    public const float PointChartValueLabelTextSize = 50f;
#endif
}
