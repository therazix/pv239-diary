﻿using Diary.Clients.Interfaces;
using Diary.Mappers;
using Diary.Models.Entry;
using Diary.Models.Mood;
using Diary.Repositories.Interfaces;
using static Diary.Enums.EntryFilterEnums;

namespace Diary.Clients;
public class EntryClient : IEntryClient
{
    private readonly IEntryRepository _repository;

    public EntryClient(IEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<ICollection<EntryListModel>> GetAllAsync(EntryFilter? entryFilter = null)
    {
        var entities = await _repository.GetAllAsync();

        foreach (var entity in entities)
        {
            entity.Title = !string.IsNullOrEmpty(entity.Title) ? entity.Title : "No title";
        }

        if (entryFilter?.OrderByProperty != null)
        {
            var orderByPropertyName = GetEnumDisplayName(entryFilter.OrderByProperty);

            if (entryFilter.OrderByDirection == OrderByDirection.Desc)
            {
                entities = entities.OrderByDescending(e => e.GetType().GetProperty(orderByPropertyName)).ToList();
            }
            else
            {
                entities = entities.OrderBy(e => e.GetType().GetProperty(orderByPropertyName)).ToList();
            }
        }

        return entities.MapToListModels();
    }

    public async Task<EntryDetailModel?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity?.MapToDetailModel();
    }

    public async Task<Guid> SetAsync(EntryDetailModel model)
    {
        var entity = model.MapToEntity();
        return await _repository.SetAsync(entity);
    }

    public async Task DeleteAsync(EntryDetailModel model)
    {
        var entity = model.MapToEntity();
        await _repository.DeleteAsync(entity);
    }

    public async Task<ICollection<MoodListModel>> GetMoodFromAllEntries()
    {
        var entities = await _repository.GetAllAsync();
        return entities.MapToMoodListModels();
    }

    public async Task<ICollection<MoodListModel>> GetMoodFromEntriesByDateRange(DateTime dateFrom, DateTime dateTo)
    {
        var entities = await _repository.GetEntriesByDateRange(dateFrom, dateTo);
        return entities.MapToMoodListModels();
    }
}
