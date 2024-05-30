using Diary.Entities;
using Diary.Models.Label;
using Riok.Mapperly.Abstractions;

namespace Diary.Mappers;

[Mapper]
public static partial class LabelMapper
{
    [MapProperty(nameof(LabelEntity.Color), nameof(LabelDetailModel.Color), Use = nameof(MapColorStringToColor))]
    [MapProperty(nameof(LabelDetailModel.Name), nameof(LabelEntity.Name), Use = nameof(MapEmptyTitleToDefaultTitle))]
    public static partial LabelDetailModel MapToDetailModel(this LabelEntity entity);
    public static partial ICollection<LabelDetailModel> MapToDetailModels(this ICollection<LabelEntity> entities);

    [MapProperty(nameof(LabelEntity.Color), nameof(LabelListModel.Color), Use = nameof(MapColorStringToColor))]
    [MapProperty(nameof(LabelEntity.Name), nameof(LabelListModel.Name), Use = nameof(MapEmptyTitleToDefaultTitle))]
    public static partial LabelListModel MapToListModel(this LabelEntity entity);
    public static partial ICollection<LabelListModel> MapToListModels(this ICollection<LabelEntity> entities);

    [MapProperty(nameof(LabelListModel.Color), nameof(LabelEntity.Color), Use = nameof(MapColorToColorString))]
    [MapperIgnoreTarget(nameof(LabelEntity.Entries))]
    [MapperIgnoreTarget(nameof(LabelEntity.Templates))]
    public static partial LabelEntity MapToEntity(this LabelListModel model);

    [MapProperty(nameof(LabelDetailModel.Color), nameof(LabelEntity.Color), Use = nameof(MapColorToColorString))]
    public static partial LabelEntity MapToEntity(this LabelDetailModel model);


    [UserMapping(Default = false)]
    private static string MapColorToColorString(Color color) => color.ToArgbHex(includeAlpha: true);

    [UserMapping(Default = false)]
    private static Color MapColorStringToColor(string color) => Color.FromArgb(color);

    [UserMapping(Default = false)]
    private static string MapEmptyTitleToDefaultTitle(string? title) => string.IsNullOrEmpty(title) ? "No title" : title;
}
