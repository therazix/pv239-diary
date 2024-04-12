using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Models.Entry;
using Diary.Models.Label;
using System.Collections.ObjectModel;

namespace Diary.ViewModels.Entry;
public partial class EntryCreateViewModel : ViewModelBase
{
    private readonly IEntryClient _entryClient;
    private readonly ILabelClient _labelClient;

    public EntryDetailModel? Entry { get; set; }

    public ObservableCollection<LabelListModel> Labels { get; set; }

    public ObservableCollection<object> SelectedLabels { get; set; }

    public EntryCreateViewModel(IEntryClient entryClient, ILabelClient labelClient)
    {
        _entryClient = entryClient;
        _labelClient = labelClient;
        Labels = new ObservableCollection<LabelListModel>();
        SelectedLabels = new ObservableCollection<object>();
    }

    public override async Task OnAppearingAsync()
    {
        Entry = new EntryDetailModel()
        {
            Id = Guid.Empty
        };

        var labels = await _labelClient.GetAllAsync();
        Labels = labels.ToObservableCollection();
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
}
