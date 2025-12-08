using System.Collections.ObjectModel;
using Ixjok.Models.Note;
using Ixjok.Services.Auth;
using SQLite;

namespace Ixjok.Services.Repository;

/// <summary>
/// Almacén dinámico local que sincroniza notas con un repositorio hospedado cuando el usuario está autenticado.
/// Implementa <see cref="IDynamicStorage"/> y mantiene una colección observable de notas.
/// </summary>
/// <remarks>
/// Suscribe a los cambios de autenticación para conectar/desconectar automáticamente con el host.
/// </remarks>
public sealed partial class DynamicStorage : IDynamicStorage
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="DynamicStorage"/>.
    /// </summary>
    /// <param name="hostedNoteRepository">Repositorio remoto de notas.</param>
    /// <param name="authentication">Manejador de autenticación para controlar sincronización.</param>
    public DynamicStorage(IHostedNoteRepository hostedNoteRepository, IBearerAuthenticationHandler authentication)
    {
        _hostedNoteRepository = hostedNoteRepository;
        _authentication = authentication;
        _authentication.AuthenticationChange += Connectivity;
        InitAsync().ConfigureAwait(false);
    }
    /// <summary>
    /// Callback para manejar cambios en el estado de autenticación.
    /// Conecta o desconecta la sincronización con el host según el estado.
    /// </summary>
    /// <param name="state">Estado de autenticación actualizado.</param>
    private async void Connectivity(AuthenticationState state) => _ = state switch
    {
        AuthenticationState.Connected => ConnectToHostAsync(),
        _ => DesConnectToHostAsync()
    };

    /// <summary>
    /// Inicializa la conexión a la base de datos SQLite local y carga las notas existentes.
    /// </summary>
    private async Task InitAsync()
    {
        if (_connection is not null) return;
        _connection = new(ConfigHelper.SqlDatabasePath, ConfigHelper.SqlFlags);
        var result = await _connection.CreateTableAsync<NoteSqlDecorator>();
        if (result == CreateTableResult.Migrated)
        {
            Notes = new(_connection!.Table<NoteSqlDecorator>().Where(x => x.Status != (int)NoteState.Deleted).OrderBy(x => x.Registration).ToArrayAsync().GetAwaiter().GetResult().Select(x => x.ToModel()));
        }
    }
    /// <summary>
    /// Elimina una nota local y sincroniza la eliminación con el host cuando corresponde.
    /// </summary>
    /// <param name="model">Nota a eliminar.</param>
    /// <param name="token">Token de cancelación opcional.</param>
    /// <returns>Tarea que representa la operación.</returns>
    public async Task RemoveAsync(NoteModel model, CancellationToken token = default)
    {
        // await InitAsync();
        var deco = model.ToDecorator();
        if (await _authentication.AuthenticationStateAsync(token) && (await _hostedNoteRepository.DeleteAsync(model.Id!.Value, token)).IsSuccess)
            deco.Status = (int)NoteState.Cloud;
        _ = _connection!.DeleteAsync(model.ToDecorator());
        Notes.Remove(model);
    }
    /// <summary>
    /// Agrega una nota local y la sube al host si el usuario está autenticado.
    /// </summary>
    /// <param name="model">Nota a agregar.</param>
    /// <param name="token">Token de cancelación opcional.</param>
    /// <returns>Tarea que representa la operación.</returns>
    public async Task AddAsync(NoteModel model, CancellationToken token = default)
    {
        // await InitAsync();
        var deco = model.ToDecorator();
        if (await _authentication.AuthenticationStateAsync(token) && (await _hostedNoteRepository.AddAsync(model, token)).IsSuccess)
            deco.Status = (int)NoteState.Cloud;
        _ = _connection!.InsertAsync(deco);
        Notes.Add(model);
    }
    /// <summary>
    /// Devuelve la colección observable de notas locales.
    /// </summary>
    /// <returns>ObservableCollection con todas las notas cargadas.</returns>
    public ObservableCollection<NoteModel> GetObservableAll()
    {
        InitAsync().Wait();
        return Notes;
    }
    /// <summary>
    /// Actualiza una nota local y sincroniza los cambios con el host cuando corresponda.
    /// </summary>
    /// <param name="model">Nota con los cambios.</param>
    /// <param name="token">Token de cancelación opcional.</param>
    /// <returns>Tarea que representa la operación.</returns>
    public async Task UpdateAsync(NoteModel model, CancellationToken token = default)
    {
        // await InitAsync();
        var deco = model.ToDecorator();
        deco.Status = (int)NoteState.Update;
        if (await _authentication.AuthenticationStateAsync(token) && (await _hostedNoteRepository.UpdateAsync(model, token)).IsSuccess)
            deco.Status = (int)NoteState.Cloud;
        _ = _connection!.UpdateAsync(deco);
    }
    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Notes.Clear();
#pragma warning disable CS8601 // Posible asignación de referencia nula
                _authentication.AuthenticationChange -= Connectivity;
#pragma warning restore CS8601 // Posible asignación de referencia nula
            }
        }
    }
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    /// <summary>
    /// Limpia el almacenamiento local y desconecta la sincronización con el host.
    /// </summary>
    /// <param name="token">Token de cancelación opcional.</param>
    /// <returns>Resultado de la operación.</returns>
    private async Task<Result> DesConnectToHostAsync(CancellationToken token = default)
    {
        Notes.Clear();
        await _connection!.DeleteAllAsync<NoteSqlDecorator>();
        return Result.Success();
    }
    /// <summary>
    /// Conecta con el repositorio hospedado y sincroniza notas hacia el almacenamiento local.
    /// </summary>
    /// <param name="token">Token de cancelación opcional.</param>
    /// <returns>Resultado de la operación.</returns>
    public async Task<Result> ConnectToHostAsync(CancellationToken token = default)
    {
        Notes.Clear();
        await foreach (var item in _hostedNoteRepository.GetAllAsync(token))
        {
            var deco = item.ToDecorator();
            deco.Status = (int)NoteState.Cloud;
            _ = _connection!.InsertAsync(deco);
            Notes.Add(item);
        }
        return Result.Success();
    }
    /// <summary>
    /// Colección observable de notas cargadas localmente.
    /// </summary>
    private ObservableCollection<NoteModel> Notes { get; set; } = [];
    /// <summary>
    /// Conexión asíncrona a la base de datos SQLite local.
    /// </summary>
    private SQLiteAsyncConnection? _connection;
    /// <summary>
    /// Indica si ya fue liberado el objeto.
    /// </summary>
    private readonly bool _disposedValue = false;
    /// <summary>
    /// Repositorio hospedado usado para sincronización remota.
    /// </summary>
    private readonly IHostedNoteRepository _hostedNoteRepository;
    /// <summary>
    /// Manejador de autenticación para validar operaciones remotas.
    /// </summary>
    private readonly IBearerAuthenticationHandler _authentication;
}