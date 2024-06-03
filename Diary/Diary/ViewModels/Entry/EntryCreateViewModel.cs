using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Enums;
using Diary.Helpers;
using Diary.Models.Entry;
using Diary.Models.Label;
using Diary.Models.Media;
using Diary.Models.Template;
using Diary.Services;
using Diary.ViewModels.Map;
using Diary.ViewModels.Media;
using System.Collections.ObjectModel;

namespace Diary.ViewModels.Entry;

[QueryProperty(nameof(PredefinedDateTime), "dateTime")]
public partial class EntryCreateViewModel : ViewModelBase
{
    private readonly IEntryClient _entryClient;
    private readonly ILabelClient _labelClient;
    private readonly ITemplateClient _templateClient;
    private readonly IMediaClient _mediaClient;
    private readonly IPopupService _popupService;
    private readonly IMediaPicker _mediaPicker;

    public TemplateDetailModel? SelectedTemplate { get; set; }
    public EntryDetailModel? Entry { get; set; }
    public ObservableCollection<LabelListModel> Labels { get; set; }
    public ObservableCollection<object> SelectedLabels { get; set; }
    public ObservableCollection<TemplateDetailModel> Templates { get; set; }
    public DateTime SelectedDate { get; set; } = DateTime.Now;
    public TimeSpan SelectedTime { get; set; } = DateTime.Now.TimeOfDay;
    public bool UseCurrentDateTime { get; set; } = true;
    public DateTime PredefinedDateTime { get; set; }

    public bool IsLocationSet { get; set; } = false;
    public string LocationText { get; set; } = string.Empty;
    public Color LocationTextColor { get; set; } = Color.FromArgb("#FF000000");

    public EntryCreateViewModel(
        IEntryClient entryClient,
        ILabelClient labelClient,
        ITemplateClient templateClient,
        IMediaClient mediaClient,
        IPopupService popupService,
        IMediaPicker mediaPicker
        )
    {
        _entryClient = entryClient;
        _labelClient = labelClient;
        _templateClient = templateClient;
        _mediaClient = mediaClient;
        _popupService = popupService;
        _mediaPicker = mediaPicker;

        Labels = new ObservableCollection<LabelListModel>();
        SelectedLabels = new ObservableCollection<object>();
        Templates = new ObservableCollection<TemplateDetailModel>();
    }

    public override async Task OnAppearingAsync()
    {
        using var _ = new BusyIndicator(this);

        Entry = new EntryDetailModel()
        {
            Id = Guid.Empty
        };

        if (PredefinedDateTime != DateTime.MinValue)
        {
            SelectedDate = PredefinedDateTime.Date;
            SelectedTime = PredefinedDateTime.TimeOfDay;
            UseCurrentDateTime = false;
        }

        var labels = await _labelClient.GetAllAsync();
        Labels = labels.ToObservableCollection();

        var templates = await _templateClient.GetAllDetailedAsync();
        Templates = templates.ToObservableCollection();

        UpdateFormLocationInfo();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (Entry != null)
        {
            using var _ = new BusyIndicator(this);

            SaveSelectedDateTime();
            Entry.Labels = new ObservableCollection<LabelListModel>(SelectedLabels.Select(l => (LabelListModel)l));
            await _entryClient.SetAsync(Entry);
        }
        await Shell.Current.GoToAsync("../");
    }

    [RelayCommand]
    private async Task DisplayMediaPopupAsync(Guid id)
    {
        await _popupService.ShowPopupAsync<MediaPopupViewModel>(onPresenting: async viewModel => await viewModel.InitializeAsync(id));
    }

    [RelayCommand]
    private async Task AddImageAsync()
    {
        var fileResult = await _mediaPicker.PickPhotoAsync();
        await LoadMediaAsync(fileResult, MediaType.Image);
    }

    [RelayCommand]
    private async Task AddVideoAsync()
    {
        var fileResult = await _mediaPicker.PickVideoAsync();
        await LoadMediaAsync(fileResult, MediaType.Video);
    }

    [RelayCommand]
    private void RemoveMedia(string fileName)
    {
        if (Entry != null)
        {
            var media = Entry.Media.FirstOrDefault(m => m.FileName == fileName);
            if (media != null)
            {
                _mediaClient.DeleteIfUnusedAsync(media);
                Entry.Media.Remove(media);
            }
        }
    }

    [RelayCommand]
    private void ClearLocation()
    {
        if (Entry != null)
        {
            Entry.Latitude = null;
            Entry.Longitude = null;
        }
        UpdateFormLocationInfo();
    }

    [RelayCommand]
    private async Task DisplayMapPopupAsync()
    {
        Location? userLocation = null;
        if (await Helpers.LocationHelper.HasLocationPermissionAsync())
        {
            userLocation = await Helpers.LocationHelper.GetAnyLocationAsync();
        }

        Location? pinLocation = null;
        if (Entry != null && Entry.Latitude != null && Entry.Longitude != null)
        {
            pinLocation = new Location((double)Entry.Latitude, (double)Entry.Longitude);
        }

        var result = await _popupService.ShowPopupAsync<MapPopupViewModel>(onPresenting: viewModel => viewModel.Initialize(pinLocation, userLocation));

        if (Entry != null && result is Location locationResult)
        {
            Entry.Latitude = locationResult.Latitude;
            Entry.Longitude = locationResult.Longitude;
        }
        UpdateFormLocationInfo();
    }

    [RelayCommand]
    private async Task InsertTemplateContentAsync()
    {
        if (!await ConfirmSelectionIfDataLossPossibleAsync())
        {
            return;
        }

        if (Entry != null && SelectedTemplate != null)
        {
            Entry.Content = SelectedTemplate.Content;

            if (SelectedTemplate.Mood != 0)
            {
                Entry.Mood = SelectedTemplate.Mood;
            }
            if (SelectedTemplate.Latitude != null)
            {
                Entry.Latitude = SelectedTemplate.Latitude.Value;
            }
            if (SelectedTemplate.Longitude != null)
            {
                Entry.Longitude = SelectedTemplate.Longitude.Value;
            }
            if (SelectedTemplate.Labels.Count > 0)
            {
                SelectedLabels = new ObservableCollection<object>(Labels.Where(l => SelectedTemplate.Labels.Select(el => el.Id).Contains(l.Id)));
            }
        }
        UpdateFormLocationInfo();
    }

    [RelayCommand]
    private void ToggleFavorite()
    {
        if (Entry != null)
        {
            Entry.IsFavorite = !Entry.IsFavorite;
        }
    }

    private void SaveSelectedDateTime()
    {
        if (Entry != null)
        {
            if (UseCurrentDateTime)
            {
                Entry.DateTime = DateTime.Now;
            }
            else
            {
                Entry.DateTime = new DateTime(SelectedDate.Year, SelectedDate.Month, SelectedDate.Day, SelectedTime.Hours, SelectedTime.Minutes, SelectedTime.Seconds);
            }
        }
    }

    private async Task<bool> ConfirmSelectionIfDataLossPossibleAsync()
    {
        bool hasEntryContent = !string.IsNullOrWhiteSpace(Entry?.Content);
        bool hasTemplateContent = !string.IsNullOrWhiteSpace(SelectedTemplate?.Content);
        bool hasTemplateMood = SelectedTemplate?.Mood != 0;
        bool hasTemplateLocation = SelectedTemplate?.Longitude != null || SelectedTemplate?.Latitude != null;
        bool hasTemplateLabels = SelectedTemplate?.Labels.Count > 0;

        if ((hasEntryContent && hasTemplateContent) || hasTemplateMood || hasTemplateLocation || hasTemplateLabels)
        {
            List<string> inputsToReplace = new();

            if (hasEntryContent)
            {
                inputsToReplace.Add("content");
            }
            if (hasTemplateMood)
            {
                inputsToReplace.Add("mood");
            }
            if (hasTemplateLocation)
            {
                inputsToReplace.Add("location");
            }
            if (hasTemplateLabels)
            {
                inputsToReplace.Add("labels");
            }

            inputsToReplace = inputsToReplace.Where(i => !string.IsNullOrWhiteSpace(i)).ToList();

            string alertText = $"Selecting a template will replace current {string.Join(", ", inputsToReplace)}.\n\nDo you want to select a template?";
            bool selectionConfirmed = await Shell.Current.CurrentPage.DisplayAlert("Template selection confirmation", alertText, "Yes", "No");

            return selectionConfirmed;
        }

        return true;
    }

    private void UpdateFormLocationInfo()
    {
        IsLocationSet = Entry != null && (Entry.Latitude != null || Entry.Longitude != null);
        LocationText = IsLocationSet ? "Set" : "None";
        // TODO: Use a converter or predefined colors
        LocationTextColor = IsLocationSet ? Color.FromArgb("#FF1B9100") : Color.FromArgb("#FF000000");
    }

    private async Task LoadMediaAsync(FileResult? fileResult, MediaType mediaType)
    {
        if (Entry != null && fileResult != null)
        {
            using var _ = new BusyIndicator(this);
            var fileName = await MediaFileService.SaveAsync(fileResult);
            if (!Entry.Media.Select(i => i.FileName).Contains(fileName))
            {
                var media = new MediaModel()
                {
                    FileName = fileName,
                    OriginalFileName = fileResult.FileName,
                    MediaType = mediaType
                };
                media = await _mediaClient.SetIfNewAsync(media);
                Entry.Media.Add(media);
            }
        }
    }
}
