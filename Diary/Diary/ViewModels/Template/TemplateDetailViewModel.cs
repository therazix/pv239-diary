using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Models.Template;
using PropertyChanged;

namespace Diary.ViewModels.Template;

[QueryProperty(nameof(Id), "id")]
public partial class TemplateDetailViewModel : ViewModelBase
{
    private readonly ITemplateClient _templateClient;

    [DoNotNotify]
    public Guid Id { get; set; }

    [ObservableProperty]
    private bool _showMood;

    [ObservableProperty]
    private bool _showLocation;

    [ObservableProperty]
    private bool _showLabels;

    public TemplateDetailModel? Template { get; set; }

    public TemplateDetailViewModel(ITemplateClient templateClient)
    {
        _templateClient = templateClient;
    }

    public override async Task OnAppearingAsync()
    {
        Template = await _templateClient.GetByIdAsync(Id);

        ShowMood = Template?.Mood != 0;
        ShowLocation = Template?.Latitude != null && Template?.Longitude != null && Template.Altitude != null;
        ShowLabels = Template?.Labels.Count > 0;
    }

    [RelayCommand]
    private async Task GoToEditAsync()
    {
        if (Template != null)
        {
            await Shell.Current.GoToAsync("/edit", new Dictionary<string, object> { ["template"] = Template });
        }
    }
}
