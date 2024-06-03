using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace Diary.Converters
{
    public class IntToMoodEmojiConverter : BaseConverterOneWay<int, string>
    {
        private static readonly string _defaultMoodEmoji = ":(";

        public override string ConvertFrom(int value, CultureInfo? culture = null)
        {
            return value switch
            {
                5 => ":D",
                4 => ":)",
                3 => ":|",
                2 => ":/",
                1 => ":(",
                _ => _defaultMoodEmoji,
            };
        }

        public override string DefaultConvertReturnValue { get; set; } = _defaultMoodEmoji;
    }
}
