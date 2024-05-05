﻿using Diary.Clients.Interfaces;
using Diary.Entities;
using Diary.Mappers;
using Diary.Models.Entry;
using Diary.Models.Mood;
using Diary.Models.Pin;
using Diary.Repositories.Interfaces;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using static Diary.Enums.EntryFilterEnums;

namespace Diary.Clients;
public class EntryClient : IEntryClient
{
    private readonly IEntryRepository _entryRepository;
    private readonly IMediaRepository _mediaRepository;

    public EntryClient(IEntryRepository entryRepository, IMediaRepository mediaRepository)
    {
        _entryRepository = entryRepository;
        _mediaRepository = mediaRepository;
    }

    public async Task<ICollection<EntryListModel>> GetAllAsync(EntryFilter? entryFilter = null)
    {
        var entities = await _entryRepository.GetAllAsync();

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

    public async Task<ICollection<EntryListModel>> GetByDayFromPreviousYears(DateTime date)
    {
        var entities = await _entryRepository.GetByDayFromPreviousYearsAsync(date);

        foreach (var entity in entities)
        {
            entity.Title = !string.IsNullOrEmpty(entity.Title) ? entity.Title : "No title";
        }

        entities = entities.OrderByDescending(e => e.CreatedAt).ToList();

        return entities.MapToListModels();
    }

    public async Task<EntryDetailModel?> GetByIdAsync(Guid id)
    {
        var entity = await _entryRepository.GetByIdAsync(id);
        return entity?.MapToDetailModel();
    }

    public async Task<EntryDetailModel> SetAsync(EntryDetailModel model)
    {
        var entity = model.MapToEntity();
        var savedEntity = await _entryRepository.SetAsync(entity);
        await _mediaRepository.DeleteUnusedMediaAsync();

#if ANDROID
        await ScheduleTimeMachineNotification(savedEntity);
#endif
        return savedEntity.MapToDetailModel();
    }

    public async Task DeleteAsync(EntryDetailModel model)
    {
        var entity = model.MapToEntity();

        await _entryRepository.DeleteAsync(entity);
        await _mediaRepository.DeleteUnusedMediaAsync();

        var entriesWithTheSameTimeMachineNotificationId = await _entryRepository.GetByTimeMachineNotificationIdAsync(entity.TimeMachineNotificationId);

        if (entriesWithTheSameTimeMachineNotificationId.Count == 0)
        {
            LocalNotificationCenter.Current.Cancel(entity.TimeMachineNotificationId);
        }
    }

    public async Task<ICollection<MoodListModel>> GetMoodFromAllEntries()
    {
        var entities = await _entryRepository.GetAllAsync();
        return entities.MapToMoodListModels();
    }

    public async Task<ICollection<MoodListModel>> GetMoodFromEntriesByDateRange(DateTime dateFrom, DateTime dateTo)
    {
        var entities = await _entryRepository.GetEntriesByDateRangeAsync(dateFrom, dateTo);
        return entities.MapToMoodListModels();
    }

    public async Task<ICollection<PinModel>> GetAllLocationPinsAsync()
    {
        var entities = await _entryRepository.GetAllAsync();
        var entitiesWithLocation = entities.Where(e => e.Latitude != null && e.Longitude != null).ToList();
        return entitiesWithLocation.MapToPinModels();
    }

    private async Task ScheduleTimeMachineNotification(EntryEntity entity)
    {
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        {
            await LocalNotificationCenter.Current.RequestNotificationPermission();
        }

        var entriesWithTheSameTimeMachineNotificationId = await _entryRepository.GetByTimeMachineNotificationIdAsync(entity.TimeMachineNotificationId);

        TimeSpan repeatInterval;
        DateTime notificationDate = entity.CreatedAt.AddYears(1);

        if (entity.CreatedAt.Month == 2 && entity.CreatedAt.Day == 29)
        {
            notificationDate = entity.CreatedAt.AddYears(4);
            repeatInterval = TimeSpan.FromDays(365 * 3 + 366);
        }
        else
        {
            repeatInterval = notificationDate - entity.CreatedAt;
        }

        var request = new NotificationRequest
        {
            NotificationId = entity.TimeMachineNotificationId,
            Title = "Diary Time Machine",
            Description = $"You have already written {entriesWithTheSameTimeMachineNotificationId.Count} entries on this day in the past.",

            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = notificationDate,
                NotifyRepeatInterval = repeatInterval,
                RepeatType = NotificationRepeat.TimeInterval
            },
            Android = { Priority = AndroidPriority.High }
        };

        // Remove scheduled notification with out-of-date number of diary entries written on the same day
        LocalNotificationCenter.Current.Cancel([entity.TimeMachineNotificationId]);

        await LocalNotificationCenter.Current.Show(request);
    }
}
