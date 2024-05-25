namespace Diary.Services.Interfaces;

public interface IImportExportService
{
    Task<MemoryStream> ExportAsync();
    Task ImportAsync(string filePath);
}
