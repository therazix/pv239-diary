using System.Globalization;

namespace Diary.Converters;

public class MediaNameToPathConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string fileName)
        {
            return Path.Combine(Constants.MediaPath, Path.GetFileName(fileName));
        }
        return string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string filePath)
        {
            return Path.GetFileName(filePath);
        }
        return string.Empty;
    }
}
