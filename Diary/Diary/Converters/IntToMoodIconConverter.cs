using CommunityToolkit.Maui.Converters;
using Diary.Resources.Fonts;
using System.Globalization;

namespace Diary.Converters;

public class IntToMoodIconConverter : BaseConverterOneWay<int, string>
{
    private static readonly string _defaultMoodIconName = "";

    public override string ConvertFrom(int value, CultureInfo? culture = null)
    {
        return value switch
        {
            5 => FontAwesomeIcons.LaughBeam,
            4 => FontAwesomeIcons.Smile,
            3 => FontAwesomeIcons.Meh,
            2 => FontAwesomeIcons.Frown,
            1 => FontAwesomeIcons.SadTear,
            _ => _defaultMoodIconName,
        };
    }

    public override string DefaultConvertReturnValue { get; set; } = _defaultMoodIconName;
}
