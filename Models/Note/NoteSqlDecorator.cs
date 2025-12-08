using SQLite;

namespace Ixjok.Models.Note;

/// <summary>
/// Adaptador ORM para persistencia de notas en SQLite.
/// Mapea las propiedades a columnas en la tabla 'notes'.
/// </summary>
[Table("notes")]
public sealed class NoteSqlDecorator
{
    /// <summary>
    /// Obtiene o establece la clave primaria (GUID) de la nota.
    /// Generada automáticamente usando UUID v7 si no se proporciona.
    /// </summary>
    [Column("id"), PrimaryKey]
    public string? Key { get; set; } = Guid.CreateVersion7().ToString();

    /// <summary>
    /// Obtiene o establece el título de la nota.
    /// Esta columna tiene un índice para optimizar búsquedas.
    /// </summary>
    [Column("title"), Indexed]
    public string? Title { get; set; }

    /// <summary>
    /// Obtiene o establece el contenido de la nota.
    /// </summary>
    [Column("content")]
    public string? Content { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha de registro en formato string.
    /// </summary>
    [Column("registration")]
    public string? Registration { get; set; }

    /// <summary>
    /// Obtiene o establece el estado de la nota como valor entero (<see cref="NoteState"/>).
    /// Por defecto es <see cref="NoteState.Local"/>.
    /// </summary>
    [Column("status")]
    public int Status { get; set; } = (int)NoteState.Local;
}