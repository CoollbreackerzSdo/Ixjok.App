namespace Ixjok.Services.Auth;

/// <summary>
/// Extiende <see cref="IAuthentication"/> con capacidad de refrescar tokens de autenticación Bearer.
/// Utiliza JWT (JSON Web Tokens) para la autenticación.
/// </summary>
public interface IBearerAuthenticationHandler : IAuthentication
{
    /// <summary>
    /// Refresca el token de autenticación Bearer de forma asíncrona.
    /// Prolonga la sesión del usuario sin requerir credenciales.
    /// </summary>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Un resultado que indica éxito o fallo de la renovación.</returns>
    Task<Result> SignRefreshAsync(CancellationToken token = default);
}