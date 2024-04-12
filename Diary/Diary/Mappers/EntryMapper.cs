﻿using Diary.Entities;
using Diary.Models.Entry;
using Riok.Mapperly.Abstractions;

namespace Diary.Mappers;

[Mapper]
public static partial class EntryMapper
{
    public static partial EntryDetailModel MapToDetailModel(this EntryEntity entity);
    public static partial ICollection<EntryDetailModel> MapToDetailModels(this ICollection<EntryEntity> entities);

    public static partial EntryListModel MapToListModel(this EntryEntity entity);
    public static partial ICollection<EntryListModel> MapToListModels(this ICollection<EntryEntity> entities);

    [MapperIgnoreTarget(nameof(EntryEntity.Content))]
    [MapperIgnoreTarget(nameof(EntryEntity.Mood))]
    [MapperIgnoreTarget(nameof(EntryEntity.Latitude))]
    [MapperIgnoreTarget(nameof(EntryEntity.Longitude))]
    [MapperIgnoreTarget(nameof(EntryEntity.Altitude))]
    [MapperIgnoreTarget(nameof(EntryEntity.Labels))]
    public static partial EntryEntity MapToEntity(this EntryListModel model);
    public static partial EntryEntity MapToEntity(this EntryDetailModel model);
}