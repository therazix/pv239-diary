using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace Diary.Converters;

public class IsEntryFavoriteToStarColorConverter : BaseConverterOneWay<bool, Color>
{
    private static Color _defaultColor = Colors.LightGray;
    private static Color _defaultColorFavorite = Colors.Yellow;

    public override Color ConvertFrom(bool value, CultureInfo? culture = null)
    {
        if (Application.Current != null)
        {
            if (Application.Current.Resources.TryGetValue("Gray500", out object? defaultGrayColor))
            {
                _defaultColor = (Color)defaultGrayColor;
            }

            if (Application.Current.Resources.TryGetValue("Tertiary", out object? defaultTertiaryColor))
            {
                _defaultColorFavorite = (Color)defaultTertiaryColor;
            }
        }

        return value ? _defaultColorFavorite : _defaultColor;
    }

    public override Color DefaultConvertReturnValue { get; set; } = _defaultColor;
}
