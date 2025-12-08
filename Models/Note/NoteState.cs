namespace Ixjok.Models.Note;

/// <summary>
/// Define los estados posibles de una nota en el sistema.
/// </summary>
public enum NoteState
{
    /// <summary>
    /// La nota solo existe localmente en el dispositivo.
    /// </summary>
    Local = 0,

    /// <summary>
    /// La nota ha sido marcada para eliminación.
    /// </summary>
    Deleted = 1,

    /// <summary>
    /// La nota ha sido actualizada y necesita sincronización.
    /// </summary>
    Update = 2,

    /// <summary>
    /// La nota está sincronizada con el servidor en la nube.
    /// </summary>
    Cloud = 3
}