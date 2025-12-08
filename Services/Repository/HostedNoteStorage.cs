using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using Ixjok.Models.Note;
using Ixjok.Tools.Common.Models.Page;

namespace Ixjok.Services.Repository;

/// <summary>
/// Cliente de almacenamiento remoto para notas que delega operaciones a un servicio HTTP.
/// Implementa <see cref="IHostedNoteRepository"/> y realiza llamadas REST para CRUD de notas.
/// </summary>
/// <remarks>
/// Utiliza <see cref="Connectivity"/> para validar acceso a internet antes de cada petición.
/// </remarks>
public sealed partial class HostedNoteStorage(HttpClient client) : IHostedNoteRepository
{
    /// <summary>
    /// Agrega una nota al almacenamiento hospedado mediante una petición POST.
    /// </summary>
    /// <param name="model">Modelo de nota a agregar.</param>
    /// <param name="token">Token de cancelación opcional.</param>
    /// <returns>Resultado de la operación indicando éxito o tipo de error.</returns>
    public async Task<Result> AddAsync(NoteModel model, CancellationToken token = default)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            return Result.Canceled();

        var result = await _client.PostAsJsonAsync("notes/complex", new
        {
            id = model.Id.Value,
            title = model.Title,
            content = model.Content,
            registration = model.Registration
        }, token);
        return result.StatusCode switch
        {
            HttpStatusCode.OK => Result.Success(),
            HttpStatusCode.BadRequest => Result.BadRequest(),
            HttpStatusCode.NotFound => Result.NoFound(),
            HttpStatusCode.Unauthorized => Result.UnAuthorized(),
            _ => Result.TeaBreak()
        };
    }
    /// <summary>
    /// Recupera todas las notas del servidor en forma de secuencia asíncrona paginada.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación para la enumeración.</param>
    /// <returns>Secuencia asíncrona de <see cref="NoteModel"/> recuperadas del servidor.</returns>
    public async IAsyncEnumerable<NoteModel> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            yield break;

        var query = new PageRequest(0, 50, PageOrder.Asc);
        var isNotComplete = true;
        while (isNotComplete)
        {
            var result = await _client.PostAsJsonAsync("notes/page", query, cancellationToken);
            if (!result.IsSuccessStatusCode) break;

            var currentIterations = 0;
            await foreach (var item in result.Content.ReadFromJsonAsAsyncEnumerable<NoteModel>(cancellationToken))
            {
                if (item is null)
                {
                    isNotComplete = false;
                    break;
                }
                currentIterations++;
                yield return item;
            }
            if (currentIterations < query.Take)
                break;
            query = query with { Skip = query.Skip + 50 };
            continue;
        }
    }
    /// <summary>
    /// Elimina una nota en el servidor por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la nota a eliminar.</param>
    /// <param name="token">Token de cancelación opcional.</param>
    /// <returns>Resultado indicando éxito o fallo de la operación.</returns>
    public async Task<Result> DeleteAsync(Guid id, CancellationToken token = default)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            return Result.Canceled();
        var result = await _client.DeleteAsync($"notes/{id}", token);
        return result.IsSuccessStatusCode || result.StatusCode == HttpStatusCode.NotFound ? Result.Success() : Result.NoFound();
    }
    /// <summary>
    /// Actualiza una nota existente en el servidor mediante una petición PUT.
    /// </summary>
    /// <param name="model">Modelo de nota con los cambios a aplicar.</param>
    /// <param name="token">Token de cancelación opcional.</param>
    /// <returns>Resultado de la operación.</returns>
    public async Task<Result> UpdateAsync(NoteModel model, CancellationToken token = default)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            return Result.Canceled();
        var result = await _client.PutAsJsonAsync($"notes", new { Id = model.Id.Value, model.Title, model.Content }, token);
        return result.IsSuccessStatusCode ? Result.Success() : Result.NoContent();
    }
    /// <summary>
    /// Libera recursos administrados si es necesario.
    /// </summary>
    /// <param name="disposing">Indica si se deben liberar recursos administrados.</param>
    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (!_isDisposing)
            {

            }
            _isDisposing = true;
        }
    }
    /// <summary>
    /// Dispose público que asegura la liberación de recursos y suprime el finalizador.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    /// <summary>
    /// Indica si el objeto está en proceso de liberación.
    /// </summary>
    private bool _isDisposing = false;
    /// <summary>
    /// Cliente HTTP usado para las llamadas al servicio remoto.
    /// </summary>
    private readonly HttpClient _client = client;
}