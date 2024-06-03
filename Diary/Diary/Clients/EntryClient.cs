using Diary.Clients.Interfaces;
using Diary.Entities;
using Diary.Mappers;
using Diary.Models.Entry;
using Diary.Models.Label;
using Diary.Models.Mood;
using Diary.Models.Pin;
using Diary.Repositories.Interfaces;
using Diary.Services;
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

    public async Task<ICollection<EntryListModel>> GetAllAsync(EntryFilterModel? entryFilter = null)
    {
        var entities = await _entryRepository.GetAllAsync();

        if (entryFilter?.FavoriteOnly == true)
        {
            entities = entities.Where(e => e.IsFavorite).ToList();
        }

        if (entryFilter?.LabelsToShow?.Count > 0)
        {
            entities = entities
                .Where(e => e.Labels.Any(l => entryFilter.LabelsToShow.Select(lts => ((LabelListModel)lts).Id).Contains(l.Id)))
                .ToList();
        }

        if (entryFilter?.MoodsToShow?.Count > 0)
        {
            entities = entities
                .Where(e => entryFilter.MoodsToShow.Select(mts => ((MoodSelectionModel)mts).Mood).Contains(e.Mood))
                .ToList();
        }

        if (entryFilter?.DateFrom != null)
        {
            entities = entities.Where(e => e.DateTime >= entryFilter.DateFrom).ToList();
        }

        if (entryFilter?.DateTo != null)
        {
            entities = entities.Where(e => e.DateTime < entryFilter.DateTo.Value.AddDays(1)).ToList();
        }

        if (entryFilter?.OrderByProperty != null)
        {
            var orderByPropertyName = GetEnumDisplayName(entryFilter.OrderByProperty);

            if (entryFilter.OrderByDirection == OrderByDirection.Descending)
            {
                entities = entities.OrderByDescending(e => e.GetType().GetProperty(orderByPropertyName)?.GetValue(e)).ToList();
            }
            else
            {
                entities = entities.OrderBy(e => e.GetType().GetProperty(orderByPropertyName)?.GetValue(e)).ToList();
            }
        }

        return entities.MapToListModels();
    }

    public async Task<ICollection<EntryListModel>> GetByDayFromPreviousYearsAsync(DateTime date)
    {
        var entities = await _entryRepository.GetByDayFromPreviousYearsAsync(date);
        entities = entities.OrderByDescending(e => e.DateTime).ToList();

        return entities.MapToListModels();
    }

    public async Task<EntryDetailModel?> GetByIdAsync(Guid id)
    {
        var entity = await _entryRepository.GetByIdAsync(id);
        return entity?.MapToDetailModel();
    }

    /// <summary>
    /// Sets the entry in the database. If the entry already exists, it will be updated.
    /// If the entry is linked to any media, the links will be updated.
    /// Media files are also updated to reflect the changes.
    /// </summary>
    public async Task<EntryDetailModel> SetAsync(EntryDetailModel model)
    {
        DateTime? oldDateTime = null;
        if (model.Id != Guid.Empty)
        {
            var oldEntity = await _entryRepository.GetByIdAsync(model.Id);
            oldDateTime = oldEntity?.DateTime;
        }

        var entity = model.MapToEntity();
        var savedEntity = await _entryRepository.SetAsync(entity);
        await _mediaRepository.DeleteIfUnusedAsync(entity.Media);
        await MediaFileService.DeleteUnusedFilesAsync(_mediaRepository);

#if ANDROID
        await ScheduleTimeMachineNotificationAsync(savedEntity, oldDateTime);
#endif
        return savedEntity.MapToDetailModel();
    }

    /// <summary>
    /// Deletes the entry from database. If the entry is linked to any media, the links will be removed.
    /// Media files are also updated to reflect the changes.
    /// </summary>
    public async Task DeleteAsync(EntryDetailModel model)
    {
        var entity = model.MapToEntity();

        await _entryRepository.DeleteAsync(entity);
        await _mediaRepository.DeleteIfUnusedAsync(entity.Media);
        await MediaFileService.DeleteUnusedFilesAsync(_mediaRepository);

        var entriesWithTheSameNotificationId = await _entryRepository.GetByNotificationIdAsync(entity.NotificationId);

        if (entriesWithTheSameNotificationId.Count == 0)
        {
            LocalNotificationCenter.Current.Cancel(entity.NotificationId);
        }
    }

    public async Task<ICollection<MoodListModel>> GetMoodFromAllEntriesAsync()
    {
        var entities = await _entryRepository.GetAllAsync();
        return entities.MapToMoodListModels();
    }

    public async Task<ICollection<MoodListModel>> GetMoodFromEntriesByDateRangeAsync(DateTime dateFrom, DateTime dateTo)
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

    private async Task ScheduleTimeMachineNotificationAsync(EntryEntity entity, DateTime? oldDateTime)
    {
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        {
            await LocalNotificationCenter.Current.RequestNotificationPermission();
        }

        if (oldDateTime != null) // If the entry is being updated, the old notification must be updated or removed
        {
            var oldNotificationId = Helpers.NotificationHelper.GetNotificationIdFromCreationDate(oldDateTime.Value);
            var entriesWithOldNotificationId = await _entryRepository.GetByNotificationIdAsync(oldNotificationId);

            LocalNotificationCenter.Current.Cancel(oldNotificationId);

            if (entriesWithOldNotificationId.Count > 0)
            {
                var previousNotification = CreateNotification(oldNotificationId, oldDateTime.Value, entriesWithOldNotificationId.Count);
                await LocalNotificationCenter.Current.Show(previousNotification);
            }
        }

        var entriesWithTheSameNotificationId = await _entryRepository.GetByNotificationIdAsync(entity.NotificationId);

        LocalNotificationCenter.Current.Cancel(entity.NotificationId);

        var request = CreateNotification(entity.NotificationId, entity.DateTime, entriesWithTheSameNotificationId.Count);

        await LocalNotificationCenter.Current.Show(request);
    }

    private NotificationRequest CreateNotification(int notificationId, DateTime date, int numOfEntries)
    {
        TimeSpan interval;

        if (date.Month == 2 && date.Day == 29)
        {
            while (date < DateTime.Now) // Notification date must be in the future
            {
                date = date.AddYears(4);
            }
            interval = TimeSpan.FromDays(365 * 3 + 366);
        }
        else
        {
            while (date < DateTime.Now) // Notification date must be in the future
            {
                date = date.AddYears(1);
            }
            interval = TimeSpan.FromDays(365);
        }


        var entryText = numOfEntries > 1 ? "entries" : "entry";

        return new NotificationRequest
        {
            NotificationId = notificationId,
            Title = "Diary Time Machine",
            Description = $"You have already written {numOfEntries} {entryText} on this day in the past.",

            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = date,
                NotifyRepeatInterval = interval,
                RepeatType = NotificationRepeat.TimeInterval
            },
            Android = { Priority = AndroidPriority.High }
        };
    }
}
