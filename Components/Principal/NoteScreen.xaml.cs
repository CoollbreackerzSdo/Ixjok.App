using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Ixjok.Components.Sign;
using Ixjok.Models.Note;
using Ixjok.Services.Auth;

namespace Ixjok.Components.Principal;

/// <summary>
/// Pantalla principal que muestra la lista de notas y permite crear, editar y eliminar notas.
/// Se vincula con <see cref="NoteViewModel"/> para gestionar la lógica de presentación.
/// </summary>
public sealed partial class NoteScreen : ContentPage
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="NoteScreen"/>.
    /// </summary>
    /// <param name="model">El ViewModel de notas inyectado por el contenedor de DI.</param>
    public NoteScreen(NoteViewModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}

/// <summary>
/// ViewModel para la pantalla principal de notas.
/// Gestiona la lista de notas locales, sincronización en la nube y autenticación.
/// </summary>
public sealed partial class NoteViewModel : BaseViewModel, IDisposable
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="NoteViewModel"/>.
    /// </summary>
    /// <param name="storage">Repositorio de almacenamiento dinámico para notas.</param>
    /// <param name="authentication">Servicio de autenticación.</param>
    /// <param name="navigation">Gestor de navegación.</param>
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

    /// <summary>
    /// Inicializa la pantalla cargando las notas almacenadas y actualizando la visibilidad del estado vacío.
    /// </summary>
    private async Task InitAsync()
    {
        IsLoading = true;
        await Task.Delay(TimeSpan.FromMilliseconds(200));
        Notes = _storage.GetObservableAll();
        Notes.CollectionChanged += UpdateEmptyVisibility;
        IsNotEmpty = Notes.Any();
        IsLoading = false;
    }

    /// <summary>
    /// Comando para refrescar la lista de notas.
    /// </summary>
    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshEnable = false;
    }

    /// <summary>
    /// Comando para alternar entre iniciar sesión y cerrar sesión.
    /// Si el usuario no está autenticado, navega a la pantalla de inicio de sesión.
    /// Si está autenticado, cierra la sesión.
    /// </summary>
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

    /// <summary>
    /// Comando para navegar a la pantalla de edición de una nota existente.
    /// </summary>
    /// <param name="model">La nota a editar.</param>
    [RelayCommand]
    private async Task GotoUpdateAsync(NoteModel model)
        => await _navigation.GoToAsync(nameof(NoteEditorScreen), query: new Dictionary<string, object?>()
        {
            { nameof(EditorNoteViewModel.Model) , model }
        });

    /// <summary>
    /// Comando para eliminar una nota.
    /// </summary>
    /// <param name="model">La nota a eliminar.</param>
    [RelayCommand]
    private async Task DeleteAsync(NoteModel model) => await _storage.RemoveAsync(model);

    /// <summary>
    /// Comando para navegar a la pantalla de creación de una nueva nota.
    /// </summary>
    [RelayCommand]
    private async Task GotoAddAsync() => await _navigation.GoToAsync(nameof(NoteEditorScreen));

    /// <summary>
    /// Actualiza el estado de sincronización en la nube cuando el estado de autenticación cambia.
    /// </summary>
    /// <param name="state">El nuevo estado de autenticación.</param>
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

    /// <summary>
    /// Actualiza la visibilidad del estado vacío cuando la colección de notas cambia.
    /// </summary>
    private async void UpdateEmptyVisibility(object? sender, NotifyCollectionChangedEventArgs e) => IsNotEmpty = Notes.Any();

    /// <summary>
    /// Libera los recursos no administrados.
    /// </summary>
    /// <param name="disposing">Indica si se deben liberar recursos administrados.</param>
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

    /// <summary>
    /// Libera todos los recursos.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Obtiene o establece si la pantalla está cargando datos.
    /// </summary>
    [ObservableProperty]
    public partial bool IsLoading { get; set; } = false;

    /// <summary>
    /// Obtiene o establece la colección observable de notas.
    /// </summary>
    [ObservableProperty]
    public partial ObservableCollection<NoteModel> Notes { get; set; }

    /// <summary>
    /// Obtiene o establece si hay notas en la colección.
    /// </summary>
    [ObservableProperty]
    public partial bool IsNotEmpty { get; set; }

    /// <summary>
    /// Obtiene o establece si el botón de refresco está habilitado.
    /// </summary>
    [ObservableProperty]
    public partial bool IsRefreshEnable { get; set; }

    /// <summary>
    /// Obtiene o establece la imagen que indica el estado de sincronización en la nube.
    /// </summary>
    [ObservableProperty]
    public partial ImageSource CloudStatusImage { get; set; }

    /// <summary>
    /// Indica si el usuario está actualmente conectado a la nube.
    /// </summary>
    private bool IsConnected { get; set; } = false;

    private bool _disposedValue;
    private readonly IDynamicStorage _storage;
    private readonly IAuthentication _authentication;
}