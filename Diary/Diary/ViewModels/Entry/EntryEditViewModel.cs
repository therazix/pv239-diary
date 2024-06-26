﻿using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Enums;
using Diary.Helpers;
using Diary.Models.Entry;
using Diary.Models.Label;
using Diary.Models.Media;
using Diary.Services;
using Diary.ViewModels.Map;
using Diary.ViewModels.Media;
using System.Collections.ObjectModel;

namespace Diary.ViewModels.Entry;

[QueryProperty(nameof(Entry), "entry")]
public partial class EntryEditViewModel : ViewModelBase
{
    private readonly IEntryClient _entryClient;
    private readonly ILabelClient _labelClient;
    private readonly IMediaClient _mediaClient;
    private readonly IPopupService _popupService;
    private readonly IMediaPicker _mediaPicker;

    private ICollection<MediaModel> _mediaToDelete;

    public EntryDetailModel Entry { get; set; } = null!;
    public ObservableCollection<LabelListModel> Labels { get; set; }
    public ObservableCollection<object> SelectedLabels { get; set; }
    public DateTime SelectedDate { get; set; } = DateTime.Now;
    public TimeSpan SelectedTime { get; set; } = DateTime.Now.TimeOfDay;
    public bool UseCurrentDateTime { get; set; }

    public bool IsLocationSet { get; set; } = false;
    public string LocationText { get; set; } = string.Empty;
    public Color LocationTextColor { get; set; } = Color.FromArgb("#FF000000");

    public EntryEditViewModel(IEntryClient entryClient, ILabelClient labelClient, IMediaClient mediaClient, IPopupService popupService, IMediaPicker mediaPicker)
    {
        _entryClient = entryClient;
        _labelClient = labelClient;
        _mediaClient = mediaClient;
        _popupService = popupService;
        _mediaPicker = mediaPicker;

        _mediaToDelete = new List<MediaModel>();

        SelectedLabels = new ObservableCollection<object>();
        Labels = new ObservableCollection<LabelListModel>();
    }

    public override async Task OnAppearingAsync()
    {
        using var _ = new BusyIndicator(this);

        SelectedDate = Entry.DateTime.Date;
        SelectedTime = Entry.DateTime.TimeOfDay;

        var labels = await _labelClient.GetAllAsync();
        SelectedLabels = new ObservableCollection<object>(labels.Where(l => Entry.Labels.Select(el => el.Id).Contains(l.Id)));
        Labels = labels.ToObservableCollection();
        UpdateFormLocationInfo();
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (Entry.Id != Guid.Empty)
        {
            await _entryClient.DeleteAsync(Entry);
        }

        await Shell.Current.GoToAsync("//entries");
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        using (new BusyIndicator(this))
        {
            SaveSelectedDateTime();

            _mediaToDelete = _mediaToDelete.Where(m => Entry.Media.All(em => em.Id != m.Id)).ToList();
            await _mediaClient.DeleteIfUnusedAsync(_mediaToDelete, [Entry.Id]);
            _mediaToDelete.Clear();

            Entry.Labels = new ObservableCollection<LabelListModel>(SelectedLabels.Select(l => (LabelListModel)l));
            await _entryClient.SetAsync(Entry);
        }

        await Shell.Current.GoToAsync("//entries");
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
                if (!_mediaToDelete.Any(m => m.Id == media.Id))
                {
                    _mediaToDelete.Add(media);
                }
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
        if (await LocationHelper.HasLocationPermissionAsync())
        {
            userLocation = await LocationHelper.GetAnyLocationAsync();
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
    private void ToggleFavorite()
    {
        if (Entry != null)
        {
            Entry.IsFavorite = !Entry.IsFavorite;
        }
    }

    private void SaveSelectedDateTime()
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
