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
public sealed partial class EditorNoteViewModel(IWorkSpace work, INavigationManager navigation) : BaseViewModel(navigation), IQueryAttributable
{
    [RelayCommand]
    private async Task Back() => await _navigation.BackAsync();
    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Title)) return;
        else if (BaseUpdateModel is null)
        {
            var result = await work.NoteRepository.AddAsync(new()
            {
                Content = Content!,
                Title = Title!,
                Registration = DateTimeOffset.Now
            });
            if (result.IsSuccess)
            {
                _ = Toast.Make("Nota Guardada").Show();
                await _navigation.BackAsync();
            }
            else if (result.Status == ResultStatus.TeaBreak && work.Mode == StorageMode.Cloud)
            {
                _ = Toast.Make("Error al conectar con el servidor").Show();
            }
        }
        else
        {
            BaseUpdateModel.Content = Content ?? string.Empty;
            BaseUpdateModel.Title = Title;
            var result = await work.NoteRepository.UpdateAsync(BaseUpdateModel);
            if (result.IsSuccess)
            {
                _ = Toast.Make("Cambios Guardados", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
                await _navigation.BackAsync();
            }
        }
    }
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(BaseUpdateModel), out var model) && model is NoteModel value)
        {
            Content = value.Content;
            Title = value.Title;
            BaseUpdateModel = value;
        }
    }
    public NoteModel? BaseUpdateModel { get; set; }
    [ObservableProperty]
    public partial string? Title { get; set; }
    [ObservableProperty]
    public partial string? Content { get; set; }
}