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

    public TemplateDetailModel? Template { get; set; }

    public TemplateDetailViewModel(ITemplateClient templateClient)
    {
        _templateClient = templateClient;
    }

    public override async Task OnAppearingAsync()
    {
        Template = await _templateClient.GetByIdAsync(Id);
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
