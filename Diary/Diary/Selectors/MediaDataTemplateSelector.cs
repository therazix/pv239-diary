using Diary.Enums;
using Diary.Models.Media;

namespace Diary.Selectors;

public class MediaDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate? ImageTemplate { get; set; }
    public DataTemplate? VideoTemplate { get; set; }

    protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
    {
        if (item is MediaModel mediaModel)
        {
            switch (mediaModel.MediaType)
            {
                case MediaType.Image:
                    return ImageTemplate;
                case MediaType.Video:
                    return VideoTemplate;
            }
        }

        return null;
    }
}
