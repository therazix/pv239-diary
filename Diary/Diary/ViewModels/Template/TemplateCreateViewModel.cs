using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Models.Label;
using Diary.Models.Template;
using System.Collections.ObjectModel;

namespace Diary.ViewModels.Template;

public partial class TemplateCreateViewModel : ViewModelBase
{
    private readonly ITemplateClient _templateClient;
    private readonly ILabelClient _labelClient;

    public bool PresetMood { get; set; } = true;

    public bool PresetLocation { get; set; } = true;

    public TemplateDetailModel? Template { get; set; }

    public ObservableCollection<LabelListModel> Labels { get; set; }

    public ObservableCollection<object> SelectedLabels { get; set; }

    public TemplateCreateViewModel(ITemplateClient templateClient, ILabelClient labelClient)
    {
        _templateClient = templateClient;
        _labelClient = labelClient;
        Labels = new ObservableCollection<LabelListModel>();
        SelectedLabels = new ObservableCollection<object>();
    }

    public override async Task OnAppearingAsync()
    {
        Template = new TemplateDetailModel()
        {
            Id = Guid.Empty
        };

        var labels = await _labelClient.GetAllAsync();
        Labels = labels.ToObservableCollection();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (Template != null)
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
        }
        await Shell.Current.GoToAsync("../");
    }
}