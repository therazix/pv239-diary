using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Models.Entry;
using Diary.Models.Label;
using System.Collections.ObjectModel;

namespace Diary.ViewModels.Entry;

[QueryProperty(nameof(Entry), "entry")]
public partial class EntryEditViewModel : ViewModelBase
{
    private readonly IEntryClient _entryClient;
    private readonly ILabelClient _labelClient;

    public EntryDetailModel Entry { get; set; } = null!;

    public ObservableCollection<LabelListModel> Labels { get; set; }

    public ObservableCollection<object> SelectedLabels { get; set; }

    public EntryEditViewModel(IEntryClient entryClient, ILabelClient labelClient)
    {
        _entryClient = entryClient;
        _labelClient = labelClient;
        SelectedLabels = new ObservableCollection<object>();
        Labels = new ObservableCollection<LabelListModel>();
    }

    public override async Task OnAppearingAsync()
    {
        var labels = await _labelClient.GetAllAsync();
        SelectedLabels = new ObservableCollection<object>(labels.Where(l => Entry.Labels.Select(el => el.Id).Contains(l.Id)));
        Labels = labels.ToObservableCollection();
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
        Entry.Labels = new ObservableCollection<LabelListModel>(SelectedLabels.Select(l => (LabelListModel)l));

        await _entryClient.SetAsync(Entry);
        await Shell.Current.GoToAsync("//entries");
    }
}
