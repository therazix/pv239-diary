using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace Diary.Converters;
public class StringToColorConverter : BaseConverterOneWay<string, Color>
{
    private static readonly Color _defaultColor = Colors.Gray;

    public override Color ConvertFrom(string value, CultureInfo? culture)
    {
        try
        {
            return Color.FromArgb(value);
        }
        catch
        {
            return _defaultColor;
        }
    }

    public override Color DefaultConvertReturnValue { get; set; } = _defaultColor;
}
