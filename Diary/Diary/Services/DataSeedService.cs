using Diary.Entities;
using Diary.Enums;
using Diary.Helpers;
using Diary.Repositories.Interfaces;

namespace Diary.Services;
public static class DataSeedService
{
    public static async Task SeedAsync(IEntryRepository entryRepository, ILabelRepository labelRepository, ITemplateRepository templateRepository, IMediaRepository mediaRepository)
    {
        // Seed media
        var beachMedia = await SaveFileAsync(Properties.Resources.BeachImage, "beach.jpg");
        var parkMedia = await SaveFileAsync(Properties.Resources.ParkImage, "park.jpg");

        beachMedia = await mediaRepository.SetAsync(beachMedia);
        parkMedia = await mediaRepository.SetAsync(parkMedia);

        entries[0].Media.Add(beachMedia);
        entries[3].Media.Add(parkMedia);

        // Seed labels
        foreach (var label in labels)
        {
            await labelRepository.SetAsync(label);
        }

        // Seed entries
        InsertLabelsToEntry(entries[0], [labels[2]]); // Nature
        InsertLabelsToEntry(entries[1], [labels[4]]); // Work
        InsertLabelsToEntry(entries[2], [labels[3]]); // Family
        InsertLabelsToEntry(entries[3], [labels[1], labels[2]]); // Health, Nature
        InsertLabelsToEntry(entries[4], [labels[0]]); // Entertainment
        // InsertLabelsToEntry(entries[5], []);
        InsertLabelsToEntry(entries[6], [labels[1]]); // Health
        InsertLabelsToEntry(entries[7], [labels[3]]); // Family

        foreach (var entry in entries)
        {
            await entryRepository.SetAsync(entry);
        }

        // Seed templates
        InsertLabelsToTemplate(templates[0], [labels[2]]); // Nature
        InsertLabelsToTemplate(templates[1], [labels[0]]); // Entertainment
        InsertLabelsToTemplate(templates[2], [labels[3]]); // Family

        foreach (var template in templates)
        {
            await templateRepository.SetAsync(template);
        }
    }

    private static List<EntryEntity> entries = new List<EntryEntity>()
    {
        new EntryEntity()
        {
            Id = Guid.NewGuid(),
            Title = "A Day at the Beach",
            Content = "Today, I spent the day at the beach. The weather was perfect, and the water was warm. I collected seashells and built a sandcastle.",
            DateTime = new DateTime(2024, 6, 1, 19, 21, 28),
            IsFavorite = true,
            Mood = 5,
            Latitude = 42.647362,
            Longitude = 18.091228,
            NotificationId = NotificationHelper.GetNotificationIdFromCreationDate(new DateTime(2024, 6, 1, 19, 21, 28))
        },
        new EntryEntity()
        {
            Id = Guid.NewGuid(),
            Title = "First Day at New Job",
            Content = "I started my new job today. It was overwhelming, but my colleagues were very welcoming. I'm excited about this new chapter.",
            DateTime = new DateTime(2024, 5, 3, 17, 38, 10),
            IsFavorite = false,
            Mood = 4,
            Latitude = 49.191245,
            Longitude = 16.611375,
            NotificationId = NotificationHelper.GetNotificationIdFromCreationDate(new DateTime(2024, 5, 3, 17, 38, 10))
        },
        new EntryEntity()
        {
            Id = Guid.NewGuid(),
            Title = "Family Dinner",
            Content = "We had a family dinner at Grandma's house. The food was amazing, and it was great to catch up with everyone.",
            DateTime = DateTime.Today.AddYears(-1),
            IsFavorite = true,
            Mood = 5,
            NotificationId = NotificationHelper.GetNotificationIdFromCreationDate(new DateTime(2024, 5, 14, 21, 57, 43))
        },
        new EntryEntity()
        {
            Id = Guid.NewGuid(),
            Title = "Morning Run",
            Content = "Went for a run in the park this morning. Felt great to start the day with some exercise and fresh air.",
            DateTime = new DateTime(2024, 5, 31, 12, 48, 39),
            IsFavorite = false,
            Mood = 5,
            NotificationId = NotificationHelper.GetNotificationIdFromCreationDate(new DateTime(2024, 5, 31, 12, 48, 39))
        },
        new EntryEntity()
        {
            Id = Guid.NewGuid(),
            Title = "Book Club Meeting",
            Content = "Had a lively discussion at the book club today. We talked about the themes and characters of our latest read.",
            DateTime = new DateTime(2024, 4, 24, 16, 34, 9),
            IsFavorite = true,
            Mood = 3,
            Latitude = 49.210064,
            Longitude = 16.599239,
            NotificationId = NotificationHelper.GetNotificationIdFromCreationDate(new DateTime(2024, 4, 24, 16, 34, 9))
        },
        new EntryEntity()
        {
            Id = Guid.NewGuid(),
            Title = "Lost Wallet",
            Content = "Lost my wallet today. Had to cancel all my cards and go through a lot of hassle. Not a great day.",
            DateTime = new DateTime(2024, 4, 23, 18, 57, 12),
            IsFavorite = false,
            Mood = 1,
            Latitude = 49.197792,
            Longitude = 16.643665,
            NotificationId = NotificationHelper.GetNotificationIdFromCreationDate(new DateTime(2024, 4, 23, 18, 57, 12))
        },
        new EntryEntity()
        {
            Id = Guid.NewGuid(),
            Title = "Sick Day",
            Content = "Spent the whole day in bed with a terrible cold. Missed work and felt really miserable.",
            DateTime = new DateTime(2024, 4, 26, 22, 42, 34),
            IsFavorite = false,
            Mood = 1,
            NotificationId = NotificationHelper.GetNotificationIdFromCreationDate(new DateTime(2024, 4, 26, 22, 42, 34))
        },
        new EntryEntity()
        {
            Id = Guid.NewGuid(),
            Title = "Argument with a Friend",
            Content = "Had a big argument with a close friend. Feel really upset and hope we can resolve it soon.",
            DateTime = new DateTime(2024, 6, 2, 9, 16, 36),
            IsFavorite = false,
            Mood = 2,
            NotificationId = NotificationHelper.GetNotificationIdFromCreationDate(new DateTime(2024, 6, 2, 9, 16, 36))
        }
    };


    private static List<TemplateEntity> templates = new List<TemplateEntity>()
    {
        new TemplateEntity()
        {
            Name = "Outdoor Activity",
            Content = "Had a wonderful time today doing [activity]. The weather was [weather description], and it was great to be outside. [Additional details about the activity].",
        },
        new TemplateEntity()
        {
            Name = "Book Club",
            Content = "",
            Latitude = 49.210064,
            Longitude = 16.599239
        },
        new TemplateEntity()
        {
            Name = "Social Event",
            Content = "Attended [event] today. It was [description of the event]. Enjoyed spending time with [people involved]. [Additional details about the event].",
            Mood = 4
        }
    };

    private static List<LabelEntity> labels = new List<LabelEntity>()
    {
        new LabelEntity()
        {
            Name = "Entertainment",
            Color = "#FFC2DB00" // yellow
        },
        new LabelEntity()
        {
            Name = "Health",
            Color = "#FFBA0000" // red
        },
        new LabelEntity()
        {
            Name = "Nature",
            Color = "#FF00700D" // green
        },
        new LabelEntity()
        {
            Name = "Family",
            Color = "#FF8500AD" // purple
        },
        new LabelEntity()
        {
            Name = "Work",
            Color = "#FF0038BA" // blue
        }
    };

    private static async Task<MediaEntity> SaveFileAsync(byte[] imageBytes, string fileName)
    {
        using Stream stream = new MemoryStream(imageBytes);
        var newFileName = await MediaFileService.SaveAsync(stream, fileName);
        return new MediaEntity()
        {
            FileName = newFileName,
            MediaType = nameof(MediaType.Image),
            OriginalFileName = fileName
        };
    }

    private static void InsertLabelsToEntry(EntryEntity entity, ICollection<LabelEntity> labels)
    {
        foreach (var label in labels)
        {
            entity.Labels.Add(label);
        }
    }

    private static void InsertLabelsToTemplate(TemplateEntity entity, ICollection<LabelEntity> labels)
    {
        foreach (var label in labels)
        {
            entity.Labels.Add(label);
        }
    }
}
