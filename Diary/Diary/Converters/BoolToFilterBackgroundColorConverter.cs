using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace Diary.Converters
{
    public class BoolToFilterBackgroundColorConverter : BaseConverterOneWay<bool, Color>
    {
        private static readonly Color _defaultColor = Color.FromArgb("#FF014FB3");

        public override Color ConvertFrom(bool value, CultureInfo? culture = null)
        {
            if (!value) return _defaultColor;

            return Color.FromArgb("#FFE72183");
        }

        public override Color DefaultConvertReturnValue { get; set; } = _defaultColor;
    }
}
