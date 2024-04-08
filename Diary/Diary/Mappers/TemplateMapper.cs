using Diary.Entities;
using Diary.Models.Template;
using Riok.Mapperly.Abstractions;

namespace Diary.Mappers;

[Mapper]
public static partial class TemplateMapper
{
    public static partial TemplateDetailModel MapToDetailModel(this TemplateEntity entity);
    public static partial ICollection<TemplateDetailModel> MapToDetailModels(this ICollection<TemplateEntity> entities);

    public static partial TemplateListModel MapToListModel(this TemplateEntity entity);
    public static partial ICollection<TemplateListModel> MapToListModels(this ICollection<TemplateEntity> entities);

    [MapperIgnoreTarget(nameof(TemplateEntity.Mood))]
    [MapperIgnoreTarget(nameof(TemplateEntity.Latitude))]
    [MapperIgnoreTarget(nameof(TemplateEntity.Longitude))]
    [MapperIgnoreTarget(nameof(TemplateEntity.Altitude))]
    [MapperIgnoreTarget(nameof(TemplateEntity.Labels))]
    public static partial TemplateEntity MapToEntity(this TemplateListModel model);
    public static partial TemplateEntity MapToEntity(this TemplateDetailModel model);
}
