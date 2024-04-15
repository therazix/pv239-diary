using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Converters;
using Diary.Helpers;
using Diary.Models.Mood;
using Diary.ViewModels.Interfaces;
using Microcharts;
using Scrtwpns.Mixbox;

namespace Diary.ViewModels.Mood;

[INotifyPropertyChanged]
public partial class MoodListViewModel : IViewModel
{
    private readonly IEntryClient _entryClient;
    private readonly IntToMoodColorConverter _intToMoodColorConverter = new();
    private readonly IntToMoodEmojiConverter _intToMoodEmojiConverter = new();

    private DateTime _weekLineChartDayFrom;
    private DateTime _weekLineChartDayTo => _weekLineChartDayFrom.AddDays(6);

    private DateTime _monthRadarChartDayFrom => new(_monthRadarChartDayTo.Year, _monthRadarChartDayTo.Month, 1);
    private DateTime _monthRadarChartDayTo;

    [ObservableProperty]
    private DateTime _averageMoodFrom;

    [ObservableProperty]
    private DateTime _averageMoodTo;

    [ObservableProperty]
    private string _monthRadarChartDayFromText;

    [ObservableProperty]
    private LineChart _weekLineChart;

    [ObservableProperty]
    private RadarChart _monthRadarChart;

    [ObservableProperty]
    private PointChart _averageMoodPointChart;

    public MoodListViewModel(IEntryClient entryClient)
    {
        _entryClient = entryClient;
    }

    public async Task OnAppearingAsync()
    {
        _weekLineChartDayFrom = DateTime.Today.AddDays(DateTime.Today.DayOfWeek == 0 ? -6 : (int)DateTime.Today.DayOfWeek - 1);
        _monthRadarChartDayTo = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

        AverageMoodFrom = _monthRadarChartDayFrom;
        AverageMoodTo = _monthRadarChartDayTo;

        await InitializeCharts();
    }

    [RelayCommand]
    private async Task WeekLineChartPrevious()
    {
        _weekLineChartDayFrom = _weekLineChartDayFrom.AddDays(-7);

        var moodEntries = await _entryClient.GetMoodFromEntriesByDateRange(_weekLineChartDayFrom, _weekLineChartDayTo);

        WeekLineChart.Entries = GetEntriesForWeekLineChart(moodEntries);
    }

    [RelayCommand]
    private async Task WeekLineChartNext()
    {
        _weekLineChartDayFrom = _weekLineChartDayFrom.AddDays(7);

        var moodEntries = await _entryClient.GetMoodFromEntriesByDateRange(_weekLineChartDayFrom, _weekLineChartDayTo);

        WeekLineChart.Entries = GetEntriesForWeekLineChart(moodEntries);
    }

    [RelayCommand]
    private async Task MonthRadarChartPrevious()
    {
        _monthRadarChartDayTo = _monthRadarChartDayTo.AddMonths(-1);

        var moodEntries = await _entryClient.GetMoodFromEntriesByDateRange(_monthRadarChartDayFrom, _monthRadarChartDayTo);

        MonthRadarChart.Entries = GetEntriesForMonthRadarChart(moodEntries);
    }

    [RelayCommand]
    private async Task MonthRadarChartNext()
    {
        _monthRadarChartDayTo = _monthRadarChartDayTo.AddMonths(1);

        var moodEntries = await _entryClient.GetMoodFromEntriesByDateRange(_monthRadarChartDayFrom, _monthRadarChartDayTo);

        MonthRadarChart.Entries = GetEntriesForMonthRadarChart(moodEntries);
    }

    private async Task InitializeCharts()
    {
        var backgroundColorHex = ColorHelper.GetBackgroundColorForCurrentTheme().ToHex();
        var textColorHex = ColorHelper.GetTextColorForCurrentTheme().ToHex();

        var moodEntriesMonth = await _entryClient.GetMoodFromEntriesByDateRange(_monthRadarChartDayFrom, _monthRadarChartDayTo);
        var moodEntriesWeek = moodEntriesMonth
            .Where(m => m.DateTime.Date >= _weekLineChartDayFrom && m.DateTime.Date <= _weekLineChartDayTo)
            .ToList();

        WeekLineChart = new LineChart
        {
            Entries = GetEntriesForWeekLineChart(moodEntriesWeek),
            MinValue = 0,
            MaxValue = 5,
            ValueLabelOrientation = Orientation.Horizontal,
            LabelOrientation = Orientation.Horizontal,
            LabelTextSize = 12,
            BackgroundColor = SkiaSharp.SKColor.Parse(backgroundColorHex),
            LabelColor = SkiaSharp.SKColor.Parse(textColorHex)
        };

        var radarChartEntries = GetEntriesForMonthRadarChart(moodEntriesMonth);

        MonthRadarChart = new RadarChart
        {
            Entries = radarChartEntries,
            MinValue = 0,
            MaxValue = radarChartEntries.Any()
                ? radarChartEntries.Where(e => e.Value != null).Max(e => (float)e.Value!) + 0.35f // Without adding this value, some circles may not be fully visible in charts
                : 0,
            BackgroundColor = SkiaSharp.SKColor.Parse(backgroundColorHex),
            LabelColor = SkiaSharp.SKColor.Parse(textColorHex),
            BorderLineColor = SkiaSharp.SKColor.Parse(textColorHex),
        };

        var moodEntriesForAverageMood = await _entryClient.GetMoodFromEntriesByDateRange(AverageMoodFrom, AverageMoodTo);
        var pointChartEntries = GetEntriesForAverageMoodPointChart(moodEntriesForAverageMood);

        AverageMoodPointChart = new PointChart
        {
            Entries = pointChartEntries,
            MinValue = 0,
            MaxValue = 5,
            ValueLabelOrientation = Orientation.Horizontal,
            LabelOrientation = Orientation.Horizontal,
            LabelTextSize = 12,
            BackgroundColor = SkiaSharp.SKColor.Parse(backgroundColorHex),
            LabelColor = SkiaSharp.SKColor.Parse(textColorHex)
        };
    }

    private List<ChartEntry> GetEntriesForWeekLineChart(ICollection<MoodListModel> moodEntries)
    {
        var currentWeekDays = Enumerable.Range(0, 7)
            .Select(i => _weekLineChartDayFrom.AddDays(i).ToString("dd.MM."))
            .ToList();

        var lineChartEntries = new List<ChartEntry>();

        foreach (var day in currentWeekDays)
        {
            var moods = moodEntries.Where(m => m.DateTime.ToString("dd.MM.") == day);
            var moodColors = moods.Select(m => _intToMoodColorConverter.ConvertFrom(m.Mood)).ToList();

            var averageMood = moods.Any()
                ? moods.Average(m => m.Mood)
                : 0;
            var averageMoodColor = moodColors.Any()
                ? moodColors.Aggregate((color1, color2) => Color.FromInt(Mixbox.Lerp(color1.ToInt(), color2.ToInt(), 0.5f)))
                : ColorHelper.GetTextColorForCurrentTheme();

            lineChartEntries.Add(new ChartEntry((float)averageMood)
            {
                Label = day,
                Color = SkiaSharp.SKColor.Parse(averageMoodColor.ToHex())
            });
        }

        return lineChartEntries;
    }

    private List<ChartEntry> GetEntriesForMonthRadarChart(ICollection<MoodListModel> moodEntries)
    {
        var moodEntryCount = moodEntries.Count;

        // Add fake data to render empty radar chart
        if (moodEntryCount == 0)
        {
            moodEntries.Add(new MoodListModel() { DateTime = DateTime.Now, Mood = 5 });
            moodEntries.Add(new MoodListModel() { DateTime = DateTime.Now, Mood = 4 });
            moodEntries.Add(new MoodListModel() { DateTime = DateTime.Now, Mood = 3 });
            moodEntries.Add(new MoodListModel() { DateTime = DateTime.Now, Mood = 2 });
            moodEntries.Add(new MoodListModel() { DateTime = DateTime.Now, Mood = 1 });
        }

        return moodEntries
            .GroupBy(m => m.Mood)
            .OrderByDescending(g => g.Key)
            .Select(g => new ChartEntry(moodEntryCount != 0 ? g.Count() : 0)
            {
                Label = _intToMoodEmojiConverter.ConvertFrom(g.Key),
                ValueLabel = (moodEntryCount != 0 ? g.Count() : 0) + "×",
                Color = SkiaSharp.SKColor.Parse(_intToMoodColorConverter.ConvertFrom(g.Key).ToHex())
            })
            .ToList();
    }

    private List<ChartEntry> GetEntriesForAverageMoodPointChart(ICollection<MoodListModel> moodEntries)
    {
        var weekDays = Enumerable.Range(0, 7)
            .Select(i => Enum.GetName(typeof(DayOfWeek), i == 6 ? 0 : i + 1).ToString()) // TODO: find better way to start with Monday
            .ToList();

        var pointChartEntries = new List<ChartEntry>();

        foreach (var day in weekDays)
        {
            var moods = moodEntries.Where(m => m.DateTime.DayOfWeek.ToString() == day);
            var moodColors = moods.Select(m => _intToMoodColorConverter.ConvertFrom(m.Mood)).ToList();

            var averageMood = moods.Any()
                ? moods.Average(m => m.Mood)
                : 0;
            // TODO: Why is 0 green and not black??
            var averageMoodColor = moodColors.Any()
                ? moodColors.Aggregate((color1, color2) => Color.FromInt(Mixbox.Lerp(color1.ToInt(), color2.ToInt(), 0.5f)))
                : ColorHelper.GetTextColorForCurrentTheme();

            pointChartEntries.Add(new ChartEntry((float)averageMood)
            {
                Label = day,
                ValueLabel = averageMood.ToString(),
                Color = SkiaSharp.SKColor.Parse(_intToMoodColorConverter.ConvertFrom((int)averageMood).ToHex())
            });
        }

        return pointChartEntries;
    }
}