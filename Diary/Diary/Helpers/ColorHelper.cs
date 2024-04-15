namespace Diary.Helpers;

public static class ColorHelper
{
    public static Color GetBackgroundColorForCurrentTheme()
    {
        if (Application.Current != null)
        {
            if (Application.Current.RequestedTheme == AppTheme.Dark && Application.Current.Resources.TryGetValue("DarkBackground", out var darkBackgroundColor))
            {
                return (Color)darkBackgroundColor;
            }

            if (Application.Current.RequestedTheme == AppTheme.Light && Application.Current.Resources.TryGetValue("White", out var lightBackgroundColor))
            {
                return (Color)lightBackgroundColor;
            }
        }

        return Colors.White;
    }

    public static Color GetTextColorForCurrentTheme()
    {
        if (Application.Current != null)
        {
            if (Application.Current.RequestedTheme == AppTheme.Dark && Application.Current.Resources.TryGetValue("PrimaryLightText", out var darkBackgroundColor))
            {
                return (Color)darkBackgroundColor;
            }

            if (Application.Current.RequestedTheme == AppTheme.Light && Application.Current.Resources.TryGetValue("PrimaryDarkText", out var lightBackgroundColor))
            {
                return (Color)lightBackgroundColor;
            }
        }

        return Colors.White;
    }
}
