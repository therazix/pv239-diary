using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Models.Label;
using Diary.Models.Template;
using System.Collections.ObjectModel;

namespace Diary.ViewModels.Template;

[QueryProperty(nameof(Template), "template")]
public partial class TemplateEditViewModel : ViewModelBase
{
    private readonly ITemplateClient _templateClient;
    private readonly ILabelClient _labelClient;

    [ObservableProperty]
    private bool _presetMood;

    [ObservableProperty]
    private bool _presetLocation;


    public TemplateDetailModel Template { get; set; } = null!;

    public ObservableCollection<LabelListModel> Labels { get; set; }

    public ObservableCollection<object> SelectedLabels { get; set; }

    public TemplateEditViewModel(ITemplateClient templateClient, ILabelClient labelClient)
    {
        _templateClient = templateClient;
        _labelClient = labelClient;

        SelectedLabels = new ObservableCollection<object>();
        Labels = new ObservableCollection<LabelListModel>();
    }

    public override async Task OnAppearingAsync()
    {
        PresetMood = Template.Mood != 0;
        PresetLocation = Template.Latitude != null && Template.Longitude != null && Template.Altitude != null;

        var labels = await _labelClient.GetAllAsync();
        SelectedLabels = new ObservableCollection<object>(labels.Where(l => Template.Labels.Select(el => el.Id).Contains(l.Id)));
        Labels = labels.ToObservableCollection();
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (Template.Id != Guid.Empty)
        {
            await _templateClient.DeleteAsync(Template);
        }

        await Shell.Current.GoToAsync("//templates");
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        Template.Mood = PresetMood ? Template.Mood : 0;

        if (!PresetLocation)
        {
            Template.Latitude = null;
            Template.Longitude = null;
            Template.Altitude = null;
        }

        Template.Labels = new ObservableCollection<LabelListModel>(SelectedLabels.Select(l => (LabelListModel)l));

        await _templateClient.SetAsync(Template);
        await Shell.Current.GoToAsync("//templates");
    }
}
