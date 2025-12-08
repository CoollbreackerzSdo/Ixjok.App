using System.Text.Json.Serialization;
using Ixjok.Tools.Converts;

namespace Ixjok.Models.Note;

/// <summary>
/// Modelo de nota observable para vinculación de datos MVVM.
/// Implementa <see cref="INote"/> e <see cref="ObservableObject"/> para notificación de cambios.
/// </summary>
public sealed partial class NoteModel : ObservableObject, INote
{
    /// <summary>
    /// Obtiene o establece el identificador único de la nota.
    /// Se serializa usando <see cref="KeyJsonConvert"/> para JSON.
    /// Generado automáticamente como UUID v7 si no se proporciona.
    /// </summary>
    [JsonConverter(typeof(KeyJsonConvert))]
    public KeyId Id { get; set; } = Guid.CreateVersion7();

    /// <summary>
    /// Obtiene o establece el título de la nota.
    /// Propiedad observable que notifica cambios de UI.
    /// </summary>
    [ObservableProperty]
    public partial string Title { get; set; }

    /// <summary>
    /// Obtiene o establece el contenido de la nota.
    /// Propiedad observable que notifica cambios de UI.
    /// </summary>
    [ObservableProperty]
    public partial string Content { get; set; }

    /// <summary>
    /// Obtiene la fecha y hora de registro de la nota.
    /// Solo lectura, inicializada con <see cref="DateTimeOffset.Now"/>.
    /// </summary>
    public DateTimeOffset Registration { get; init; } = DateTimeOffset.Now;
}