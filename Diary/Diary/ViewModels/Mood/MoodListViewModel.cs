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

    private string _backgroundColorHex = ColorHelper.GetBackgroundColorForCurrentTheme().ToHex();
    private string _textColorHex = ColorHelper.GetTextColorForCurrentTheme().ToHex();

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

        _weekLineChartDayFrom = DateTime.Today.AddDays(DateTime.Today.DayOfWeek == 0 ? -6 : (int)DateTime.Today.DayOfWeek - 1);
        _monthRadarChartDayTo = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

        AverageMoodFrom = _monthRadarChartDayFrom;
        AverageMoodTo = _monthRadarChartDayTo;
    }

    public async Task OnAppearingAsync()
    {
        await InitializeChartsAsync();
    }

    [RelayCommand]
    private async Task WeekLineChartPrevious()
    {
        _weekLineChartDayFrom = _weekLineChartDayFrom.AddDays(-7);

        var moodEntries = await _entryClient.GetMoodFromEntriesByDateRange(_weekLineChartDayFrom, _weekLineChartDayTo);

        SetWeekLineChart(moodEntries);
    }

    [RelayCommand]
    private async Task WeekLineChartNextAsync()
    {
        _weekLineChartDayFrom = _weekLineChartDayFrom.AddDays(7);

        var moodEntries = await _entryClient.GetMoodFromEntriesByDateRange(_weekLineChartDayFrom, _weekLineChartDayTo);

        SetWeekLineChart(moodEntries);
    }

    [RelayCommand]
    private async Task MonthRadarChartPreviousAsync()
    {
        _monthRadarChartDayTo = _monthRadarChartDayTo.AddMonths(-1);

        var moodEntries = await _entryClient.GetMoodFromEntriesByDateRange(_monthRadarChartDayFrom, _monthRadarChartDayTo);

        SetMonthRadarChart(moodEntries);
    }

    [RelayCommand]
    private async Task MonthRadarChartNextAsync()
    {
        _monthRadarChartDayTo = _monthRadarChartDayTo.AddMonths(1);

        var moodEntries = await _entryClient.GetMoodFromEntriesByDateRange(_monthRadarChartDayFrom, _monthRadarChartDayTo);

        SetMonthRadarChart(moodEntries);
    }

    [RelayCommand]
    private async Task ChangeAverageMoodChartRangeAsync()
    {
        var moodEntries = await _entryClient.GetMoodFromEntriesByDateRange(AverageMoodFrom, AverageMoodTo);

        SetAverageMoodPointChart(moodEntries);
    }

    private async Task InitializeChartsAsync()
    {
        var moodEntriesMonth = await _entryClient.GetMoodFromEntriesByDateRange(_monthRadarChartDayFrom, _monthRadarChartDayTo);
        var moodEntriesWeek = moodEntriesMonth
            .Where(m => m.DateTime.Date >= _weekLineChartDayFrom && m.DateTime.Date <= _weekLineChartDayTo)
            .ToList();

        SetWeekLineChart(moodEntriesWeek);
        SetMonthRadarChart(moodEntriesMonth);

        var moodEntriesForAverageMood = await _entryClient.GetMoodFromEntriesByDateRange(AverageMoodFrom, AverageMoodTo);
        SetAverageMoodPointChart(moodEntriesForAverageMood);
    }

    private void SetWeekLineChart(ICollection<MoodListModel> moodEntries)
    {
        var lineChartEntries = GetEntriesForWeekLineChart(moodEntries);

        WeekLineChart = new LineChart
        {
            Entries = lineChartEntries,
            MinValue = -0.5f, // -0.5 is used to increase padding between point and labels, if there was any other way to increase padding, 0 would be used
            MaxValue = 5,
            ValueLabelOrientation = Orientation.Horizontal,
            LabelOrientation = Orientation.Horizontal,
            LabelTextSize = Constants.LineChartLabelTextSize,
            BackgroundColor = SkiaSharp.SKColor.Parse(_backgroundColorHex),
            LabelColor = SkiaSharp.SKColor.Parse(_textColorHex),
            LineSize = Constants.LineChartLineSize,
        };
    }

    private void SetMonthRadarChart(ICollection<MoodListModel> moodEntries)
    {
        var radarChartEntries = GetEntriesForMonthRadarChart(moodEntries);
        var maxValue = radarChartEntries.Any()
                ? radarChartEntries.Where(e => e.Value != null).Max(e => (float)e.Value!) + 0.35f // Without adding this value, some circles may not be fully visible in charts
                : 0;

        MonthRadarChart = new RadarChart
        {
            Entries = radarChartEntries,
            MinValue = 0,
            MaxValue = maxValue,
            BackgroundColor = SkiaSharp.SKColor.Parse(_backgroundColorHex),
            LabelColor = SkiaSharp.SKColor.Parse(_textColorHex),
            BorderLineColor = SkiaSharp.SKColor.Parse(_textColorHex),
            LineSize = Constants.RadarChartLineSize,
            LabelTextSize = Constants.RadarChartLabelTextSize,
        };
    }

    private void SetAverageMoodPointChart(ICollection<MoodListModel> moodEntries)
    {
        var pointChartEntries = GetEntriesForAverageMoodPointChart(moodEntries);

        AverageMoodPointChart = new PointChart
        {
            Entries = pointChartEntries,
            MinValue = -0.5f, // -0.5 is used to increase padding between point and labels, if there was any other way to increase padding, 0 would be used
            MaxValue = 5.5f, // 5.5 is used to increase padding between point and labels, if there was any other way to increase padding, 5 would be used
            ValueLabelOrientation = Orientation.Horizontal,
            LabelOrientation = Orientation.Horizontal,
            LabelTextSize = Constants.PointChartLabelTextSize,
            BackgroundColor = SkiaSharp.SKColor.Parse(_backgroundColorHex),
            LabelColor = SkiaSharp.SKColor.Parse(_textColorHex),
            PointSize = Constants.PointChartPointSize,
            ValueLabelTextSize = Constants.PointChartValueLabelTextSize,
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

            var averageMoodColor = moodColors.Any()
                ? moodColors.Aggregate((color1, color2) => Color.FromInt(Mixbox.Lerp(color1.ToInt(), color2.ToInt(), 0.5f)))
                : ColorHelper.GetTextColorForCurrentTheme();

            pointChartEntries.Add(new ChartEntry((float)averageMood)
            {
                Label = day,
                ValueLabel = averageMood != 0 ? _intToMoodEmojiConverter.ConvertFrom((int)averageMood) : "",
                ValueLabelColor = SkiaSharp.SKColor.Parse(averageMoodColor.ToHex()),
                Color = SkiaSharp.SKColor.Parse(averageMoodColor.ToHex())
            });
        }

        return pointChartEntries;
    }
}