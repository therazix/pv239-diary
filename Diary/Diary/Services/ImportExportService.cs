using Diary.Entities;
using Diary.Repositories.Interfaces;
using Diary.Services.Interfaces;
using Newtonsoft.Json;

namespace Diary.Services;
public class ImportExportService : IImportExportService
{
    private readonly IEntryRepository _entryRepository;
    private readonly ILabelRepository _labelRepository;
    private readonly ITemplateRepository _templateRepository;

    public ImportExportService(IEntryRepository entryRepository, ILabelRepository labelRepository, ITemplateRepository templateRepository)
    {
        _entryRepository = entryRepository;
        _labelRepository = labelRepository;
        _templateRepository = templateRepository;
    }

    public async Task<string> ExportAsync()
    {
        var entries = await _entryRepository.GetAllAsync();
        var labels = await _labelRepository.GetAllAsync();
        var templates = await _templateRepository.GetAllAsync();

        var export = new ImportExportEntity()
        {
            Entries = entries,
            Labels = labels,
            Templates = templates
        };

        return JsonConvert.SerializeObject(export, Formatting.Indented);
    }

    public async Task ImportAsync(string content)
    {
        var import = JsonConvert.DeserializeObject<ImportExportEntity>(content);

        if (import != null)
        {
            ReplaceIds(import);

            foreach (var label in import.Labels)
            {
                await _labelRepository.SetAsync(label);
            }

            foreach (var entry in import.Entries)
            {
                await _entryRepository.SetAsync(entry);
            }

            foreach (var template in import.Templates)
            {
                await _templateRepository.SetAsync(template);
            }
        }
    }

    private void ReplaceIdProps<T>(Dictionary<Guid, Guid> idMap, T entity) where T : EntityBase
    {
        foreach (var prop in entity.GetType().GetProperties())
        {
            if (prop.Name.ToLower().EndsWith("id") && prop.PropertyType == typeof(Guid))
            {
                var id = prop.GetValue(entity);
                if (id != null)
                {
                    if (idMap.TryGetValue((Guid)id, out var newId))
                    {
                        prop.SetValue(entity, newId);
                    }
                    else
                    {
                        newId = Guid.NewGuid();
                        idMap.Add((Guid)id, newId);
                        prop.SetValue(entity, newId);
                    }
                }
            }
        }
    }

    private void ReplaceIds(ImportExportEntity import)
    {
        var idMap = new Dictionary<Guid, Guid>();

        foreach (var label in import.Labels)
        {
            foreach (var entry in label.Entries)
            {
                ReplaceIdProps(idMap, entry);
            }
            foreach (var template in label.Templates)
            {
                ReplaceIdProps(idMap, template);
            }

            ReplaceIdProps(idMap, label);
        }

        foreach (var entry in import.Entries)
        {
            foreach (var label in entry.Labels)
            {
                ReplaceIdProps(idMap, label);
            }
            ReplaceIdProps(idMap, entry);
        }

        foreach (var template in import.Templates)
        {
            foreach (var label in template.Labels)
            {
                ReplaceIdProps(idMap, label);
            }
            ReplaceIdProps(idMap, template);
        }
    }
}
