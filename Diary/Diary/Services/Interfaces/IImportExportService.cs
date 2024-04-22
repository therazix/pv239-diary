namespace Diary.Services.Interfaces;

public interface IImportExportService
{
    Task<string> ExportAsync();
    Task ImportAsync(string content);
}
