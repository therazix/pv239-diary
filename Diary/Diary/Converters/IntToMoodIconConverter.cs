using CommunityToolkit.Maui.Converters;
using Diary.Resources.Fonts;
using System.Globalization;

namespace Diary.Converters;

public class IntToMoodIconConverter : BaseConverterOneWay<int, string>
{
    private static readonly string _defaultMoodIconName = FontAwesomeIcons.SmilePlus;

    public override string ConvertFrom(int value, CultureInfo? culture)
    {
        return value switch
        {
            1 => FontAwesomeIcons.LaughBeam,
            2 => FontAwesomeIcons.Smile,
            3 => FontAwesomeIcons.Meh,
            4 => FontAwesomeIcons.Frown,
            5 => FontAwesomeIcons.SadTear,
            _ => _defaultMoodIconName,
        };
    }

    public override string DefaultConvertReturnValue { get; set; } = _defaultMoodIconName;
}
