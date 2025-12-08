using Ixjok.Models.Note;

namespace Ixjok.Components.Editors;

/// <summary>
/// Pantalla para crear y editar notas.
/// Se vincula con <see cref="EditorNoteViewModel"/> para gestionar la lógica de edición.
/// </summary>
public partial class NoteEditorScreen : ContentPage
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="NoteEditorScreen"/>.
    /// </summary>
    /// <param name="model">El ViewModel del editor de notas inyectado por el contenedor de DI.</param>
    public NoteEditorScreen(EditorNoteViewModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}

/// <summary>
/// ViewModel para la pantalla de edición de notas.
/// Gestiona la creación de nuevas notas y la actualización de notas existentes.
/// Implementa <see cref="IQueryAttributable"/> para recibir parámetros de ruta.
/// </summary>
public sealed partial class EditorNoteViewModel(IDynamicStorage storage, INavigationManager navigation) : BaseViewModel(navigation), IQueryAttributable
{
    /// <summary>
    /// Comando para regresar a la pantalla anterior.
    /// </summary>
    [RelayCommand]
    private async Task BackAsync() => await _navigation.BackAsync();

    /// <summary>
    /// Comando para guardar la nota actual.
    /// Si es una actualización, actualiza la nota; de lo contrario, crea una nueva.
    /// </summary>
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

    /// <summary>
    /// Procesa los atributos de ruta recibidos de la navegación.
    /// Si se proporciona una nota existente, configura el ViewModel para editar.
    /// </summary>
    /// <param name="query">Diccionario de parámetros de ruta.</param>
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(Model), out var model) && model is NoteModel value)
        {
            Model = value;
            IsUpdate = true;
        }
    }

    /// <summary>
    /// Indica si el ViewModel está en modo de actualización o creación.
    /// </summary>
    private bool IsUpdate { get; set; }

    /// <summary>
    /// Obtiene o establece la nota que se está editando.
    /// </summary>
    [ObservableProperty]
    public partial NoteModel Model { get; set; } = new();

    private readonly IDynamicStorage _storage = storage;
}