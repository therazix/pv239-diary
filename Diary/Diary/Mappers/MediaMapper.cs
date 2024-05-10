using Diary.Entities;
using Diary.Enums;
using Diary.Models.Media;
using Riok.Mapperly.Abstractions;

namespace Diary.Mappers;

[Mapper]
public static partial class MediaMapper
{
    [MapProperty(nameof(MediaEntity.MediaType), nameof(MediaModel.MediaType), Use = nameof(MapMediaTypeStringToMediaType))]
    public static partial MediaModel MapToModel(this MediaEntity entity);
    public static partial ICollection<MediaModel> MapToModels(this ICollection<MediaEntity> entities);

    [MapProperty(nameof(MediaModel.MediaType), nameof(MediaEntity.MediaType), Use = nameof(MapMediaTypeToMediaTypeString))]
    public static partial MediaEntity MapToEntity(this MediaModel model);
    public static partial ICollection<MediaEntity> MapToEntites(this ICollection<MediaModel> models);


    [UserMapping(Default = false)]
    private static string MapMediaTypeToMediaTypeString(MediaType mediaType) => mediaType.ToString();

    [UserMapping(Default = false)]
    private static MediaType MapMediaTypeStringToMediaType(string mediaType) => Enum.Parse<MediaType>(mediaType);
}
