using CommunityToolkit.Maui.Converters;
using Diary.Helpers;
using System.Globalization;

namespace Diary.Converters
{
    public class IntToMoodColorConverter : BaseConverterOneWay<int, Color>
    {
        private static readonly string _defaultColorString = "#FF56E74A";
        private static readonly Color _defaultColor = ColorHelper.GetTextColorForCurrentTheme();

        public override Color ConvertFrom(int value, CultureInfo? culture = null)
        {
            return Color.FromArgb(ConvertToColorStringFrom(value));
        }

        public string ConvertToColorStringFrom(int value)
        {
            return value switch
            {
                5 => "#FF56E74A",
                4 => "#FFD5F32B",
                3 => "#FFF1B502",
                2 => "#FFE06A00",
                1 => "#FFD91E1E",
                _ => _defaultColorString,
            };
        }

        public override Color DefaultConvertReturnValue { get; set; } = _defaultColor;
    }
}
