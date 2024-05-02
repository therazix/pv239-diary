using Diary.Entities;
using Diary.Enums;
using Diary.Models.Media;
using Riok.Mapperly.Abstractions;

namespace Diary.Mappers;

[Mapper]
public static partial class MediaMapper
{
    public static MediaModel MapToModel(this MediaEntity entity)
    {
        return new MediaModel()
        {
            FileName = entity.FileName,
            MediaType = Enum.Parse<MediaType>(entity.MediaType),
        };
    }
    public static partial ICollection<MediaModel> MapToModels(this ICollection<MediaEntity> entities);

    public static MediaEntity MapToEntity(this MediaModel model)
    {
        return new MediaEntity()
        {
            FileName = model.FileName,
            MediaType = model.MediaType.ToString(),
        };
    }
    public static partial ICollection<MediaEntity> MapToEntites(this ICollection<MediaModel> models);
}
