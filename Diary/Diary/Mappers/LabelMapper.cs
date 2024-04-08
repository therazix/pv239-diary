using Diary.Entities;
using Diary.Models.Label;
using Riok.Mapperly.Abstractions;

namespace Diary.Mappers;

[Mapper]
public static partial class LabelMapper
{
    public static partial LabelDetailModel MapToDetailModel(this LabelEntity entity);
    public static partial ICollection<LabelDetailModel> MapToDetailModels(this ICollection<LabelEntity> entities);

    public static partial LabelListModel MapToListModel(this LabelEntity entity);
    public static partial ICollection<LabelListModel> MapToListModels(this ICollection<LabelEntity> entities);

    [MapperIgnoreTarget(nameof(LabelEntity.Entries))]
    [MapperIgnoreTarget(nameof(LabelEntity.Templates))]
    public static partial LabelEntity MapToEntity(this LabelListModel model);
    public static partial LabelEntity MapToEntity(this LabelDetailModel model);
}
