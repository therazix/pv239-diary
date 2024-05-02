using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.Input;
using Diary.Services.Interfaces;

namespace Diary.ViewModels.ImportExport;

public partial class ImportExportViewModel : ViewModelBase
{
    private readonly IFilePicker _filePicker;
    private readonly IFileSaver _fileSaver;
    private readonly IImportExportService _importExportService;
    private readonly FilePickerFileType _zipFileType = new(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                       {
                    { DevicePlatform.Android, new[] { "application/zip" } } ,
                    { DevicePlatform.iOS, new[] { "public.zip-archive" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.zip-archive" } },
                    { DevicePlatform.macOS, new[] { "zip" } },
                    { DevicePlatform.WinUI, new[] { ".zip" } }
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
        var stream = await _importExportService.ExportAsync();
        var fileSaverResult = await _fileSaver.SaveAsync(Constants.DefaultExportFileName, stream);

        var toast = Toast.Make(fileSaverResult.IsSuccessful ? "Successfully exported" : fileSaverResult.Exception.Message);
        await toast.Show();
    }

    [RelayCommand]
    private async Task ImportAsync()
    {
        var options = new PickOptions()
        {
            PickerTitle = "Please select a file to import",
            FileTypes = _zipFileType,
        };

        string? toastMessage = null;
        try
        {
            var result = await _filePicker.PickAsync(options);
            if (result != null)
            {
                if (result.FileName.EndsWith("zip", StringComparison.OrdinalIgnoreCase))
                {
                    await _importExportService.ImportAsync(result.FullPath);
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
