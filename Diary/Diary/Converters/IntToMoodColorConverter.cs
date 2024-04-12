using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace Diary.Converters
{
    public class IntToMoodColorConverter : BaseConverterOneWay<int, Color>
    {
        private static readonly Color _defaultColor = Color.FromArgb("#FF56E74A");

        public override Color ConvertFrom(int value, CultureInfo? culture)
        {
            return value switch
            {
                1 => Color.FromArgb("#FF56E74A"),
                2 => Color.FromArgb("#FFD5F32B"),
                3 => Color.FromArgb("#FFF1B502"),
                4 => Color.FromArgb("#FFE06A00"),
                5 => Color.FromArgb("#FFD91E1E"),
                _ => _defaultColor,
            };
        }

        public override Color DefaultConvertReturnValue { get; set; } = _defaultColor;
    }
}
