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
    public NoteViewModel(IDynamicStorage storage, IAuthentication authentication, INavigationManager navigation) : base(navigation)
    {
        _storage = storage;
        _authentication = authentication;
        Notes ??= [];
        CloudStatusImage = "cloud_disable.png";
        IsLoading = true;
        _authentication.AuthenticationChange += UpdateCloudMode;
        InitAsync().ConfigureAwait(false);
    }
    private async Task InitAsync()
    {
        IsLoading = true;
        await Task.Delay(TimeSpan.FromMilliseconds(200));
        Notes = _storage.GetObservableAll();
        Notes.CollectionChanged += UpdateEmptyVisibility;
        IsNotEmpty = Notes.Any();
        IsLoading = false;
    }
    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshEnable = false;
    }
    [RelayCommand]
    private async Task SignAsync()
    {
        if (!IsConnected)
        {
            await _navigation.GoToAsync(nameof(SignInScreen));
            return;
        }
        var result = await _authentication.SignOutAsync();
        _ = result.IsSuccess ? Toast.Make("Session Cerrada", CommunityToolkit.Maui.Core.ToastDuration.Long).Show() : Toast.Make("Error Externo", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
    }
    [RelayCommand]
    private async Task GotoUpdateAsync(NoteModel model)
        => await _navigation.GoToAsync(nameof(NoteEditorScreen), query: new Dictionary<string, object?>()
        {
            { nameof(EditorNoteViewModel.Model) , model }
        });
    [RelayCommand]
    private async Task DeleteAsync(NoteModel model) => await _storage.RemoveAsync(model);
    [RelayCommand]
    private async Task GotoAddAsync() => await _navigation.GoToAsync(nameof(NoteEditorScreen));
    private async void UpdateCloudMode(AuthenticationState state)
    {
        IsLoading = true;
        if (state == AuthenticationState.Connected)
        {
            CloudStatusImage = "cloud_enable.png";
            IsConnected = true;
        }
        else if (state == AuthenticationState.DisConnected)
        {
            CloudStatusImage = "cloud_disable.png";
            IsConnected = false;
        }
        IsLoading = false;
    }
    private async void UpdateEmptyVisibility(object? sender, NotifyCollectionChangedEventArgs e) => IsNotEmpty = Notes.Any();
    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Notes.CollectionChanged -= UpdateEmptyVisibility;
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
    public partial bool IsLoading { get; set; } = false;
    [ObservableProperty]
    public partial ObservableCollection<NoteModel> Notes { get; set; }
    [ObservableProperty]
    public partial bool IsNotEmpty { get; set; }
    [ObservableProperty]
    public partial bool IsRefreshEnable { get; set; }
    [ObservableProperty]
    public partial ImageSource CloudStatusImage { get; set; }
    private bool IsConnected { get; set; } = false;
    private bool _disposedValue;
    private readonly IDynamicStorage _storage;
    private readonly IAuthentication _authentication;
}