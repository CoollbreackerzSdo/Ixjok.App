using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Ixjok.Components.Sign;
using Ixjok.Models.Note;
using Ixjok.Services.Auth;

namespace Ixjok.Components.Principal;

public sealed partial class NoteScreen : ContentPage
{
    public NoteScreen(NoteViewModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
public sealed partial class NoteViewModel : BaseViewModel, IDisposable
{
    public NoteViewModel(IWorkSpace work, INavigationManager navigation, IAuthentication authentication) : base(navigation)
    {
        _authentication = authentication;
        _work = work;
        _work.Refresh += Refresh;
        Notes ??= _work.NoteRepository.GetObservableAll();
        IsNotEmpty = Notes.Any();
        Notes.CollectionChanged += UpdateProperty;
        CloudStatusImage = _work.Mode switch
        {
            StorageMode.Cloud => "cloud_enable.png",
            _ => "cloud_disable.png"
        };
    }
    [RelayCommand]
    private async Task RefreshAsync()
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet && _work.Mode == StorageMode.Cloud)
        {
            _ = Toast.Make("Sin Internet").Show();
            IsRefreshEnable = false;
            return;
        }
        Notes = _work.NoteRepository.GetObservableAll();
        Notes.CollectionChanged += UpdateProperty;
        IsNotEmpty = Notes.Any();
        CloudStatusImage = _work.Mode switch
        {
            StorageMode.Cloud => "cloud_enable.png",
            _ => "cloud_disable.png"
        };
        IsRefreshEnable = false;
    }
    [RelayCommand]
    private async Task SignAsync()
    {
        if (await _authentication.RequestAuthenticationStateAsync() == false)
        {
            await _navigation.GoToAsync(nameof(SignInScreen));
            return;
        }
        IsRefreshEnable = true;
        _ = _authentication.SignOutAsync();
        await _work.ChangeModeAsync(StorageMode.Default);
        await Toast.Make("Session Cerrada", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
    }
    [RelayCommand]
    private async Task GotoUpdateAsync(NoteModel model)
        => await _navigation.GoToAsync(nameof(NoteEditorScreen), query: new Dictionary<string, object?>()
        {
            { nameof(EditorNoteViewModel.BaseUpdateModel) , model }
        });
    [RelayCommand]
    private async Task DeleteAsync(NoteModel model) => await _work.NoteRepository.RemoveAsync(model);
    [RelayCommand]
    private async Task GotoAddAsync() => await _navigation.GoToAsync(nameof(NoteEditorScreen));
    private async void UpdateProperty(object? sender, NotifyCollectionChangedEventArgs e)
        => IsNotEmpty = Notes.Any();
    private async void Refresh() => await RefreshAsync();
    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Notes.CollectionChanged -= UpdateProperty;
            }
            _disposedValue = true;
        }
    }
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    [ObservableProperty]
    public partial ObservableCollection<NoteModel> Notes { get; set; }
    [ObservableProperty]
    public partial bool IsNotEmpty { get; set; }
    [ObservableProperty]
    public partial bool IsRefreshEnable { get; set; }
    [ObservableProperty]
    public partial ImageSource CloudStatusImage { get; set; }
    private bool _disposedValue;
    private readonly IAuthentication _authentication;
    private readonly IWorkSpace _work;
}