namespace Ixjok.Models.Note;

/// <summary>
/// Define la interfaz para un modelo de nota con propiedades de título y contenido.
/// Hereda de <see cref="IDateable"/> para incluir metadatos de fecha de registro.
/// </summary>
public interface INote : IDateable
{
    /// <summary>
    /// Obtiene o establece el título de la nota.
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// Obtiene o establece el contenido o cuerpo de la nota.
    /// </summary>
    string Content { get; set; }
}