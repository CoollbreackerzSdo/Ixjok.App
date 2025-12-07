using Ixjok.Models.Note;

namespace Ixjok.Components.Editors;

public partial class NoteEditorScreen : ContentPage
{
    public NoteEditorScreen(EditorNoteViewModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
public sealed partial class EditorNoteViewModel(IDynamicStorage storage, INavigationManager navigation) : BaseViewModel(navigation), IQueryAttributable
{
    [RelayCommand]
    private async Task BackAsync() => await _navigation.BackAsync();
    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsUpdate)
        {
            _ = _storage.UpdateAsync(Model);
            await BackAsync();
            return;
        }
        await _storage.AddAsync(Model);
        await BackAsync();
    }
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(Model), out var model) && model is NoteModel value)
        {
            Model = value;
            IsUpdate = true;
        }
    }
    private bool IsUpdate { get; set; }
    [ObservableProperty]
    public partial NoteModel Model { get; set; } = new();
    private readonly IDynamicStorage _storage = storage;
}