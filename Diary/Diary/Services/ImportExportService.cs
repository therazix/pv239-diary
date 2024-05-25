using Diary.Entities;
using Diary.Repositories.Interfaces;
using Diary.Services.Interfaces;
using Newtonsoft.Json;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Writers;

namespace Diary.Services;
public class ImportExportService : IImportExportService
{
    private readonly IEntryRepository _entryRepository;
    private readonly ILabelRepository _labelRepository;
    private readonly ITemplateRepository _templateRepository;
    private readonly string _importDir;
    private readonly string _importMediaDir;
    private readonly string _importDataFileName;
    private readonly string _exportDir;
    private readonly string _exportMediaDir;
    private readonly string _exportDataFileName;

    public ImportExportService(IEntryRepository entryRepository, ILabelRepository labelRepository, ITemplateRepository templateRepository)
    {
        _entryRepository = entryRepository;
        _labelRepository = labelRepository;
        _templateRepository = templateRepository;

        _importDir = Path.Combine(Constants.TempPath, "import");
        _importMediaDir = Path.Combine(_importDir, "media");
        _importDataFileName = Path.Combine(_importDir, "data.json");

        _exportDir = Path.Combine(Constants.TempPath, "export");
        _exportMediaDir = Path.Combine(_exportDir, "media");
        _exportDataFileName = Path.Combine(_exportDir, "data.json");
    }


    public async Task<MemoryStream> ExportAsync()
    {
        RecreateDirectories();
        await ExportDataFileAsync(_exportDataFileName);
        CopyMediaFiles(Constants.MediaPath, _exportMediaDir);
        return CreateZip();
    }

    public async Task ImportAsync(string filePath)
    {
        RecreateDirectories();
        ExtractZip(filePath);
        await ImportDataFileAsync(_importDataFileName);
        CopyMediaFiles(_importMediaDir, Constants.MediaPath);
    }

    private void RecreateDirectories()
    {
        if (Directory.Exists(_exportDir))
        {
            Directory.Delete(_exportDir, true);
        }
        Directory.CreateDirectory(_exportDir);

        if (Directory.Exists(_importDir))
        {
            Directory.Delete(_importDir, true);
        }
        Directory.CreateDirectory(_importDir);
    }

    private void CopyMediaFiles(string sourceDir, string targetDir)
    {
        if (!Directory.Exists(targetDir))
        {
            Directory.CreateDirectory(targetDir);
        }

        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var destFile = Path.Combine(targetDir, Path.GetFileName(file));
            File.Copy(file, destFile, true);
        }
    }

    private async Task ExportDataFileAsync(string filePath)
    {
        var entries = await _entryRepository.GetAllAsync();
        var labels = await _labelRepository.GetAllAsync();
        var templates = await _templateRepository.GetAllAsync();

        var data = new ImportExportEntity()
        {
            Entries = entries,
            Labels = labels,
            Templates = templates
        };

        using StreamWriter file = File.CreateText(filePath);
        JsonSerializer serializer = new JsonSerializer();
        serializer.Serialize(file, data);
    }

    private async Task ImportDataFileAsync(string filePath)
    {
        using StreamReader file = File.OpenText(filePath);

        JsonSerializer serializer = new JsonSerializer();
        var import = serializer.Deserialize(file, typeof(ImportExportEntity)) as ImportExportEntity;

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

    private MemoryStream CreateZip()
    {
        var memoryStream = new MemoryStream();
        using var archive = ZipArchive.Create();

        var dirName = Path.GetFileName(_exportMediaDir.TrimEnd(Path.DirectorySeparatorChar));
        foreach (var filePath in Directory.GetFiles(_exportMediaDir))
        {
            var fileName = Path.GetFileName(filePath);
            archive.AddEntry($"{dirName}\\{fileName}", filePath);
        }
        archive.AddEntry(Path.GetFileName(_exportDataFileName), _exportDataFileName);
        archive.SaveTo(memoryStream, new WriterOptions(CompressionType.Deflate)
        {
            LeaveStreamOpen = true
        });
        memoryStream.Position = 0;

        return memoryStream;
    }

    private void ExtractZip(string filePath)
    {
        using var archive = ZipArchive.Open(filePath);
        archive.ExtractToDirectory(_importDir);
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
            foreach (var media in entry.Media)
            {
                ReplaceIdProps(idMap, media);
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
