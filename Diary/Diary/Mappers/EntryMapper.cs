using Diary.Entities;
using Diary.Models.Entry;
using Diary.Models.Mood;
using Diary.Models.Pin;
using Riok.Mapperly.Abstractions;

namespace Diary.Mappers;

[Mapper]
[UseStaticMapper(typeof(LabelMapper))]
public static partial class EntryMapper
{
    public static partial EntryDetailModel MapToDetailModel(this EntryEntity entity);
    public static partial ICollection<EntryDetailModel> MapToDetailModels(this ICollection<EntryEntity> entities);

    [MapProperty(nameof(EntryEntity.Title), nameof(EntryListModel.Title), Use = nameof(MapEmptyTitleToDefaultTitle))]
    [MapProperty(nameof(EntryEntity.Content), nameof(EntryListModel.Content), Use = nameof(MapContentToContentSubstring))]
    [MapProperty(nameof(EntryEntity.Media), nameof(EntryListModel.MediaCount), Use = nameof(MapMediaToMediaCount))]
    public static partial EntryListModel MapToListModel(this EntryEntity entity);

    public static partial ICollection<EntryListModel> MapToListModels(this ICollection<EntryEntity> entities);

    [MapProperty(nameof(EntryEntity.DateTime), nameof(MoodListModel.DateTime))]
    public static partial MoodListModel MapToMoodListModel(this EntryEntity entities);

    public static partial ICollection<MoodListModel> MapToMoodListModels(this ICollection<EntryEntity> entities);

    public static PinModel MapToPinModel(this EntryEntity entity)
    {
        return new PinModel()
        {
            EntryId = entity.Id,
            Title = entity.Title,
            Description = $"Created: {entity.DateTime.ToString(Constants.MapDateTimeFormat)}",
            Location = new Location()
            {
                Latitude = entity.Latitude ?? 0,
                Longitude = entity.Longitude ?? 0,
            },
        };
    }

    public static partial ICollection<PinModel> MapToPinModels(this ICollection<EntryEntity> entities);

    [MapperIgnoreTarget(nameof(EntryEntity.Content))]
    [MapperIgnoreTarget(nameof(EntryEntity.Mood))]
    [MapperIgnoreTarget(nameof(EntryEntity.Latitude))]
    [MapperIgnoreTarget(nameof(EntryEntity.Longitude))]
    [MapperIgnoreTarget(nameof(EntryEntity.Labels))]
    [MapperIgnoreTarget(nameof(EntryEntity.Media))]
    [MapperIgnoreSource(nameof(EntryListModel.MediaCount))]
    public static partial EntryEntity MapToEntity(this EntryListModel model);

    public static partial EntryEntity MapToEntity(this EntryDetailModel model);

    [UserMapping(Default = false)]
    private static string MapEmptyTitleToDefaultTitle(string? title) => string.IsNullOrEmpty(title) ? "No title" : title;

    [UserMapping(Default = false)]
    private static string MapContentToContentSubstring(string content) => content.Length > 50 ? content[..47] + "..." : content;

    [UserMapping(Default = false)]
    private static int MapMediaToMediaCount(ICollection<MediaEntity> media) => media.Count;
}
