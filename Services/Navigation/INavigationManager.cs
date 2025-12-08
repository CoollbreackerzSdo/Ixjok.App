namespace Ixjok.Services.Navigation;

/// <summary>
/// Define el gestor de navegación para la aplicación.
/// Proporciona métodos para navegar entre pantallas y pasar parámetros de ruta.
/// </summary>
public interface INavigationManager
{
    /// <summary>
    /// Navega a la pantalla principal de forma asíncrona.
    /// </summary>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación de navegación.</returns>
    public Task GotoHomeAsync(CancellationToken token = default);

    /// <summary>
    /// Navega a una ruta especificada de forma asíncrona.
    /// </summary>
    /// <param name="route">La ruta de destino (nombre de la pantalla registrada).</param>
    /// <param name="animated">Indica si la transición debe ser animada. Por defecto es falso.</param>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación de navegación.</returns>
    public Task GoToAsync(string route, bool animated = false, CancellationToken token = default);

    /// <summary>
    /// Navega a una ruta especificada pasando parámetros de ruta de forma asíncrona.
    /// </summary>
    /// <param name="route">La ruta de destino (nombre de la pantalla registrada).</param>
    /// <param name="query">Un diccionario con los parámetros a pasar a la pantalla destino.</param>
    /// <param name="animated">Indica si la transición debe ser animada. Por defecto es falso.</param>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación de navegación.</returns>
    public Task GoToAsync(string route, IDictionary<string, Object?> query, bool animated = false, CancellationToken token = default);

    /// <summary>
    /// Regresa a la pantalla anterior de forma asíncrona.
    /// </summary>
    /// <param name="animated">Indica si la transición debe ser animada. Por defecto es falso.</param>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación de navegación hacia atrás.</returns>
    public Task BackAsync(bool animated = false, CancellationToken token = default);
}