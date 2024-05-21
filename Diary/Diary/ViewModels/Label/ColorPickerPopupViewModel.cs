namespace Diary.ViewModels.Label;

public class ColorPickerPopupViewModel : ViewModelBase
{
    public Color Color { get; set; } = Constants.DefaultLabelColor;

    public ColorPickerPopupViewModel()
    {
    }

    public void Initialize(Color color)
    {
        Color = color;
    }
}
