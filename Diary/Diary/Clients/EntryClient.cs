using Diary.Clients.Interfaces;
using Diary.Entities;
using Diary.Mappers;
using Diary.Models.Entry;
using Diary.Models.Mood;
using Diary.Models.Pin;
using Diary.Repositories.Interfaces;
using Plugin.LocalNotification;
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

    public async Task<ICollection<EntryListModel>> GetByDayFromPreviousYears(DateTime date) 
    {
        var entities = await _repository.GetByDayFromPreviousYears(date);

        foreach (var entity in entities)
        {
            entity.Title = !string.IsNullOrEmpty(entity.Title) ? entity.Title : "No title";
        }

        entities = entities.OrderByDescending(e => e.CreatedAt).ToList();

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
        var savedEntity = await _repository.SetAsync(entity);

#if ANDROID
        await ScheduleTimeMachineNotification(savedEntity);
#endif
        return savedEntity.Id;
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

    public async Task<ICollection<PinModel>> GetAllLocationPinsAsync()
    {
        var entities = await _repository.GetAllAsync();
        var entitiesWithLocation = entities.Where(e => e.Latitude != null && e.Longitude != null).ToList();
        return entitiesWithLocation.MapToPinModels();
    }

    private async Task ScheduleTimeMachineNotification(EntryEntity entity)
    {
        var entriesWithTheSameTimeMachineNotificationId = await _repository.GetByTimeMachineNotificationId(entity.TimeMachineNotificationId);

        TimeSpan repeatInterval;
        DateTime notificationDate = entity.CreatedAt.AddYears(1);

        if (entity.CreatedAt.Month == 2 && entity.CreatedAt.Day == 29)
        {
            repeatInterval = TimeSpan.FromDays(365*3 + 366);
            notificationDate = entity.CreatedAt.AddYears(4);
        }
        else if (DateTime.IsLeapYear(entity.CreatedAt.Year) && entity.CreatedAt > new DateTime(entity.CreatedAt.Year, 2, 28))
        {
            repeatInterval = TimeSpan.FromDays(366);
        }
        else
        {
            repeatInterval = TimeSpan.FromDays(365);
        }

        var request = new NotificationRequest
        {
            NotificationId = entity.TimeMachineNotificationId,
            Title = "Diary Time Machine",
            Description = $"You have already written {entriesWithTheSameTimeMachineNotificationId.Count} entries on this day in the past.",
            BadgeNumber = entriesWithTheSameTimeMachineNotificationId.Count,

            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = notificationDate,
                NotifyRepeatInterval = repeatInterval
            }
        };

        // Remove scheduled notification with out-of-date number of diary entries written on the same day
        LocalNotificationCenter.Current.Cancel([entity.TimeMachineNotificationId]);

        await LocalNotificationCenter.Current.Show(request);
    }
}
