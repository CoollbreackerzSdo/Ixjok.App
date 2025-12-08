using Ixjok.Models.Note;

namespace Ixjok.Services.Repository;

/// <summary>
/// Define un repositorio para notas hospedadas en servidor remoto.
/// Proporciona operaciones CRUD asincrónicas para notas almacenadas en la nube.
/// </summary>
public interface IHostedNoteRepository : IDisposable
{
    /// <summary>
    /// Actualiza una nota en el servidor remoto de forma asíncrona.
    /// </summary>
    /// <param name="model">La nota a actualizar.</param>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Un resultado que indica éxito o fallo de la actualización.</returns>
    Task<Result> UpdateAsync(NoteModel model, CancellationToken token = default);

    /// <summary>
    /// Elimina una nota del servidor remoto de forma asíncrona.
    /// </summary>
    /// <param name="id">El identificador de la nota a eliminar.</param>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Un resultado que indica éxito o fallo de la eliminación.</returns>
    Task<Result> DeleteAsync(Guid id, CancellationToken token = default);

    /// <summary>
    /// Agrega una nueva nota al servidor remoto de forma asíncrona.
    /// </summary>
    /// <param name="model">La nota a agregar.</param>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Un resultado que indica éxito o fallo de la adición.</returns>
    Task<Result> AddAsync(NoteModel model, CancellationToken token = default);

    /// <summary>
    /// Obtiene todas las notas del servidor remoto de forma asíncrona.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Una enumeración asíncrona de notas desde el servidor.</returns>
    IAsyncEnumerable<NoteModel> GetAllAsync(CancellationToken cancellationToken = default);
}