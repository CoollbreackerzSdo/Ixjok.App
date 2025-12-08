using Ixjok.Models.Note;

namespace Ixjok.Services.Repository;

/// <summary>
/// Define un repositorio dinámico para notas con capacidad de sincronización con un servidor remoto.
/// Extiende <see cref="IRepository{NoteModel}"/> para agregar sincronización en la nube.
/// </summary>
public interface IDynamicStorage : IRepository<NoteModel>, IDisposable
{
    /// <summary>
    /// Conecta el almacenamiento local con el servidor remoto para sincronización de notas.
    /// </summary>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Un resultado que indica éxito o fallo de la conexión.</returns>
    Task<Result> ConnectToHostAsync(CancellationToken token);
}
