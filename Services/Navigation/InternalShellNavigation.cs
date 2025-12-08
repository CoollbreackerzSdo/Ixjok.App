
namespace Ixjok.Services.Navigation;

/// <summary>
/// Implementación de <see cref="INavigationManager"/> usando la Shell de MAUI.
/// Gestiona la navegación interna entre pantallas registradas.
/// </summary>
public sealed class InternalShellNavigation : INavigationManager
{
    /// <summary>
    /// Regresa a la pantalla anterior de forma asíncrona.
    /// </summary>
    /// <param name="animated">Indica si la transición debe ser animada.</param>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación de navegación.</returns>
    public async Task BackAsync(bool animated = false, CancellationToken token = default)
        => await Shell.Current.GoToAsync("..", animated);
    /// <summary>
    /// Navega a una ruta especificada de forma asíncrona.
    /// </summary>
    /// <param name="route">La ruta de destino.</param>
    /// <param name="animated">Indica si la transición debe ser animada.</param>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación de navegación.</returns>
    public async Task GoToAsync(string route, bool animated = false, CancellationToken token = default)
         => await Shell.Current.GoToAsync(route, animated);
    /// <summary>
    /// Navega a una ruta especificada con parámetros de forma asíncrona.
    /// </summary>
    /// <param name="route">La ruta de destino.</param>
    /// <param name="query">Diccionario de parámetros para pasar a la pantalla.</param>
    /// <param name="animated">Indica si la transición debe ser animada.</param>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación de navegación.</returns>
    public async Task GoToAsync(string route, IDictionary<string, object?> query, bool animated = false, CancellationToken token = default)
        => await Shell.Current.GoToAsync(route, animated, query);
    /// <summary>
    /// Navega a la pantalla principal de forma asíncrona.
    /// </summary>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación de navegación.</returns>
    public async Task GotoHomeAsync(CancellationToken token = default)
        => await Shell.Current.GoToAsync("///Home", false);
}