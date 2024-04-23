using Diary.Entities;
using Diary.Repositories.Interfaces;

namespace Diary.Services;
public static class DataSeedService
{
    public static async Task SeedAsync(IEntryRepository entryRepository, ILabelRepository labelRepository, ITemplateRepository templateRepository)
    {
        foreach (var label in labels)
        {
            await labelRepository.SetAsync(label);
        }

        entries[0].Labels.Add(labels[0]); // Entry 1 has red label
        entries[1].Labels.Add(labels[1]);
        entries[1].Labels.Add(labels[2]); // Entry 2 has green and blue labels, Entry 3 has no labels
        foreach (var entry in entries)
        {
            await entryRepository.SetAsync(entry);
        }

        templates[0].Labels.Add(labels[2]); // Template 1 has blue label
        templates[1].Labels.Add(labels[1]); // Template 2 has green label
        templates[2].Labels.Add(labels[0]); // Template 3 has red and blue label
        templates[2].Labels.Add(labels[2]);
        foreach (var template in templates)
        {
            await templateRepository.SetAsync(template);
        }
    }

    private static List<EntryEntity> entries = new List<EntryEntity>()
    {
        new EntryEntity()
        {
            Title = "Entry 1",
            Content = "Entry Content 1",
            CreatedAt = DateTime.Now,
            EditedAt = DateTime.Now,
            IsFavorite = false,
            Mood = 1,
            Latitude = 1,
            Longitude = 1
        },
        new EntryEntity()
        {
            Title = "Entry 2",
            Content = "Entry Content 2",
            CreatedAt = DateTime.Now,
            EditedAt = DateTime.Now,
            IsFavorite = false,
            Mood = 2,
            Latitude = 2,
            Longitude = 2
        },
        new EntryEntity()
        {
            Title = "Entry 3",
            Content = "Entry Content 3",
            CreatedAt = DateTime.Now,
            EditedAt = DateTime.Now,
            IsFavorite = false,
            Mood = 3,
            Latitude = 3,
            Longitude = 3
        }
    };


    private static List<TemplateEntity> templates = new List<TemplateEntity>()
    {
        new TemplateEntity()
        {
            Name = "Template 1",
            Content = "Template Content 1",
            Mood = 1,
            Latitude = 1,
            Longitude = 1
        },
        new TemplateEntity()
        {
            Name = "Template 2",
            Content = "Template Content 2",
            Mood = 2,
            Latitude = 2,
            Longitude = 2
        },
        new TemplateEntity()
        {
            Name = "Template 3",
            Content = "Template Content 3",
            Mood = 3,
            Latitude = 3,
            Longitude = 3
        }
    };

    private static List<LabelEntity> labels = new List<LabelEntity>()
    {
        new LabelEntity()
        {
            Name = "Label 1",
            Color = "#FFFF0000" // red
        },
        new LabelEntity()
        {
            Name = "Label 2",
            Color = "#FF00FF00" // green
        },
        new LabelEntity()
        {
            Name = "Label 3",
            Color = "#FF0000FF" // blue
        }
    };
}
