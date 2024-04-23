﻿using Diary.Entities;
using Diary.Models.Entry;
using Diary.Models.Mood;
using Diary.Models.Pin;
using Riok.Mapperly.Abstractions;

namespace Diary.Mappers;

[Mapper]
public static partial class EntryMapper
{
    public static partial EntryDetailModel MapToDetailModel(this EntryEntity entity);
    public static partial ICollection<EntryDetailModel> MapToDetailModels(this ICollection<EntryEntity> entities);

    public static partial EntryListModel MapToListModel(this EntryEntity entity);
    public static partial ICollection<EntryListModel> MapToListModels(this ICollection<EntryEntity> entities);

    [MapProperty(nameof(EntryEntity.CreatedAt), nameof(MoodListModel.DateTime))]
    public static partial MoodListModel MapToMoodListModel(this EntryEntity entities);

    public static partial ICollection<MoodListModel> MapToMoodListModels(this ICollection<EntryEntity> entities);

    public static PinModel MapToPinModel(this EntryEntity entity)
    {
        return new PinModel()
        {
            EntryId = entity.Id,
            Title = entity.Title,
            Description = $"Created: {entity.CreatedAt.ToString(Constants.MapDateTimeFormat)}",
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
    public static partial EntryEntity MapToEntity(this EntryListModel model);
    public static partial EntryEntity MapToEntity(this EntryDetailModel model);
}
