using System.Globalization;

namespace Diary.Converters;

public class DateStringToCalendarFooterText : IValueConverter
{
    public static readonly string DefaultText = "All entries";

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string dateString)
        {
            return string.IsNullOrEmpty(dateString) ? DefaultText : dateString;
        }

        return DefaultText;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string dateString)
        {
            return dateString == DefaultText ? string.Empty : dateString;
        }
        return string.Empty;
    }
}
