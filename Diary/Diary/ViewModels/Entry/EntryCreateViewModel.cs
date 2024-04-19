using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Models.Entry;
using Diary.Models.Label;
using Diary.Models.Template;
using System.Collections.ObjectModel;

namespace Diary.ViewModels.Entry;

public partial class EntryCreateViewModel : ViewModelBase
{
    private readonly IEntryClient _entryClient;
    private readonly ILabelClient _labelClient;
    private readonly ITemplateClient _templateClient;

    [ObservableProperty]
    private TemplateDetailModel? _selectedTemplate;

    public EntryDetailModel? Entry { get; set; }

    public ObservableCollection<LabelListModel> Labels { get; set; }

    public ObservableCollection<object> SelectedLabels { get; set; }

    public ObservableCollection<TemplateDetailModel> Templates { get; set; }

    public EntryCreateViewModel(IEntryClient entryClient, ILabelClient labelClient, ITemplateClient templateClient)
    {
        _entryClient = entryClient;
        _labelClient = labelClient;
        _templateClient = templateClient;

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
    private async Task InsertTemplateContent()
    {
        if (!await ConfirmSelectionIfDataLossPossible())
        {
            return;
        }

        if (Entry != null && SelectedTemplate != null)
        {
            Entry.Content = SelectedTemplate.Content;

            if (SelectedTemplate.Mood != null)
            {
                Entry.Mood = SelectedTemplate.Mood.Value;
            }
            if (SelectedTemplate.Latitude != null)
            {
                Entry.Latitude = SelectedTemplate.Latitude.Value;
            }
            if (SelectedTemplate.Longitude != null)
            {
                Entry.Longitude = SelectedTemplate.Longitude.Value;
            }
            if (SelectedTemplate.Altitude != null)
            {
                Entry.Altitude = SelectedTemplate.Altitude.Value;
            }
            if (SelectedTemplate.Labels.Count > 0)
            {
                // BUG: SelectedLabels are added correctly when saving the entry, but they are not highlighted as selected in the UI
                Entry.Labels = new ObservableCollection<LabelListModel>(SelectedTemplate.Labels);
            }
        }
    }

    private async Task<bool> ConfirmSelectionIfDataLossPossible()
    {
        bool hasEntryContent = !string.IsNullOrWhiteSpace(Entry?.Content);
        bool hasTemplateContent = !string.IsNullOrWhiteSpace(SelectedTemplate?.Content);
        bool hasTemplateMood = SelectedTemplate?.Mood != null;
        bool hasTemplateLocation = SelectedTemplate?.Longitude != null || SelectedTemplate?.Latitude != null || SelectedTemplate?.Altitude != null;
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
}
