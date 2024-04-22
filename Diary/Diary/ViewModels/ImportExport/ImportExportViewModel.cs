using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.Input;
using Diary.Services.Interfaces;
using System.Text;

namespace Diary.ViewModels.ImportExport;

public partial class ImportExportViewModel : ViewModelBase
{
    private readonly IFilePicker _filePicker;
    private readonly IFileSaver _fileSaver;
    private readonly IImportExportService _importExportService;
    private readonly FilePickerFileType _jsonFileType = new(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "application/json" } } ,
                    { DevicePlatform.iOS, new[] { "public.json" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.json" } },
                    { DevicePlatform.macOS, new[] { "json" } },
                    { DevicePlatform.WinUI, new[] { ".json" } }
                });

    public ImportExportViewModel(IFilePicker filePicker, IFileSaver fileSaver, IImportExportService importExportService)
    {
        _filePicker = filePicker;
        _fileSaver = fileSaver;
        _importExportService = importExportService;
    }

    [RelayCommand]
    private async Task ExportAsync()
    {
        var export = await _importExportService.ExportAsync();
        using var stream = new MemoryStream(Encoding.Default.GetBytes(export));

        var fileSaverResult = await _fileSaver.SaveAsync(Constants.AppFolder, Constants.DefaultImportExportFileName, stream);

        var toast = Toast.Make(fileSaverResult.IsSuccessful ? "Successfully exported" : fileSaverResult.Exception.Message);
        await toast.Show();
    }

    [RelayCommand]
    private async Task ImportAsync()
    {
        var options = new PickOptions()
        {
            PickerTitle = "Please select a file to import",
            FileTypes = _jsonFileType,
        };

        string? toastMessage = null;
        try
        {
            var result = await _filePicker.PickAsync(options);
            if (result != null)
            {
                if (result.FileName.EndsWith("json", StringComparison.OrdinalIgnoreCase))
                {
                    using var stream = await result.OpenReadAsync();

                    using var reader = new StreamReader(stream);
                    var content = await reader.ReadToEndAsync();

                    await _importExportService.ImportAsync(content);

                    toastMessage = "Successfully imported";
                }
            }
        }
        catch (Exception ex)
        {
            toastMessage = ex.Message;
        }

        var toast = Toast.Make(toastMessage ?? "Import failed");
        await toast.Show();
    }
}
