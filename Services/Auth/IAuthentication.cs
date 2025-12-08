namespace Ixjok.Services.Auth;

/// <summary>
/// Define los servicios de autenticación y gestión de sesión de usuario.
/// Proporciona métodos para registrarse, iniciar sesión, cerrar sesión y verificar el estado de autenticación.
/// </summary>
public interface IAuthentication
{
    /// <summary>
    /// Obtiene o establece una acción que se invoca cuando el estado de autenticación cambia.
    /// </summary>
    public Action<AuthenticationState> AuthenticationChange { get; set; }

    /// <summary>
    /// Verifica de forma asíncrona si el usuario está actualmente autenticado.
    /// </summary>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Verdadero si el usuario está autenticado; de lo contrario, falso.</returns>
    Task<bool> AuthenticationStateAsync(CancellationToken token = default);

    /// <summary>
    /// Cierra la sesión del usuario actual de forma asíncrona.
    /// </summary>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Un resultado que indica éxito o fallo del cierre de sesión.</returns>
    Task<Result> SignOutAsync(CancellationToken token = default);

    /// <summary>
    /// Inicia sesión con las credenciales proporcionadas de forma asíncrona.
    /// </summary>
    /// <param name="request">Los datos de inicio de sesión.</param>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Un resultado que indica éxito o fallo del inicio de sesión.</returns>
    Task<Result> SignInAsync(SignInRequest request, CancellationToken token = default);

    /// <summary>
    /// Registra un nuevo usuario de forma asíncrona.
    /// </summary>
    /// <param name="request">Los datos de registro.</param>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Un resultado que indica éxito o fallo del registro.</returns>
    Task<Result> SignUpAsync(SignUpRequest request, CancellationToken token = default);
}

/// <summary>
/// Define los estados posibles de autenticación del usuario.
/// </summary>
public enum AuthenticationState
{
    /// <summary>
    /// El usuario está conectado y autenticado.
    /// </summary>
    Connected,

    /// <summary>
    /// El usuario está desconectado.
    /// </summary>
    DisConnected,

    /// <summary>
    /// El estado de autenticación es desconocido o no se ha verificado.
    /// </summary>
    None
}