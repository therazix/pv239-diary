using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Enums;
using Diary.Models.Entry;
using Diary.Models.Label;
using Diary.Models.Media;
using Diary.Models.Template;
using Diary.ViewModels.Map;
using System.Collections.ObjectModel;

namespace Diary.ViewModels.Entry;

public partial class EntryCreateViewModel : ViewModelBase
{
    private readonly IEntryClient _entryClient;
    private readonly ILabelClient _labelClient;
    private readonly ITemplateClient _templateClient;
    private readonly IMediaClient _mediaClient;
    private readonly IPopupService _popupService;
    private readonly IMediaPicker _mediaPicker;

    [ObservableProperty]
    private TemplateDetailModel? _selectedTemplate;

    public EntryDetailModel? Entry { get; set; }

    public ObservableCollection<LabelListModel> Labels { get; set; }

    public ObservableCollection<object> SelectedLabels { get; set; }

    public ObservableCollection<TemplateDetailModel> Templates { get; set; }

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
        Entry = new EntryDetailModel()
        {
            Id = Guid.Empty
        };

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
            Entry.Labels = new ObservableCollection<LabelListModel>(SelectedLabels.Select(l => (LabelListModel)l));
            await _entryClient.SetAsync(Entry);
        }
        await Shell.Current.GoToAsync("../");
    }

    [RelayCommand]
    private async Task AddImageAsync()
    {
        var fileResult = await _mediaPicker.PickPhotoAsync();
        await LoadMedia(fileResult, MediaType.Image);
    }

    [RelayCommand]
    private async Task AddVideoAsync()
    {
        var fileResult = await _mediaPicker.PickVideoAsync();
        await LoadMedia(fileResult, MediaType.Video);
    }

    [RelayCommand]
    private Task RemoveMediaAsync(string fileName)
    {
        if (Entry != null)
        {
            var media = Entry.Media.FirstOrDefault(m => m.FileName == fileName);
            if (media != null)
            {
                Entry.Media.Remove(media);
            }
        }

        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task ClearLocationAsync()
    {
        if (Entry != null)
        {
            Entry.Latitude = null;
            Entry.Longitude = null;
        }
        UpdateFormLocationInfo();
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task DisplayMapPopupAsync()
    {
        Location? userLocation = null;
        if (await Helpers.LocationHelper.HasLocationPermission())
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

    private async Task LoadMedia(FileResult? fileResult, MediaType mediaType)
    {
        if (Entry != null && fileResult != null)
        {
            var fileName = await _mediaClient.SaveFileAsync(fileResult);
            if (!Entry.Media.Select(i => i.FileName).Contains(fileName))
            {
                Entry.Media.Add(new MediaModel() { FileName = fileName, MediaType = mediaType });
            }
        }
    }
}
