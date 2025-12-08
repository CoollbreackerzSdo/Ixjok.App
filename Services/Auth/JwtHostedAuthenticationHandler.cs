using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Ixjok.Tools.Common.Models.Bearer;

namespace Ixjok.Services.Auth;

/// <summary>
/// Manejador de autenticación JWT que se comunica con un servidor remoto para operaciones de autenticación.
/// Implementa <see cref="IBearerAuthenticationHandler"/> y <see cref="IDisposable"/> para gestionar tokens Bearer y recursos.
/// </summary>
public sealed partial class JwtHostedAuthenticationHandler : IBearerAuthenticationHandler, IDisposable
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="JwtHostedAuthenticationHandler"/>.
    /// </summary>
    /// <param name="client">Cliente HTTP para comunicarse con el servidor de autenticación.</param>
    public JwtHostedAuthenticationHandler(HttpClient client)
    {
        _client = client;
        AuthenticationChange = (_) => { };
        InitAsync().ConfigureAwait(false);
    }
    /// <summary>
    /// Obtiene el estado actual de autenticación del usuario.
    /// </summary>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Un booleano que indica si el usuario está autenticado.</returns>
    public async Task<bool> AuthenticationStateAsync(CancellationToken token = default)
    {
        if (_transport is null) return false;
        await SignRefreshAsync(token);
        return _transport is not null;
    }
    /// <summary>
    /// Inicia sesión con credenciales de usuario.
    /// </summary>
    /// <param name="request">Solicitud de inicio de sesión con nombre de usuario y contraseña.</param>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Un resultado que indica éxito o error de la operación.</returns>
    public async Task<Result> SignInAsync(SignInRequest request, CancellationToken token = default)
    {
        var result = await _client.PostAsJsonAsync("auth/sign-in", request, token);
        if (result.IsSuccessStatusCode)
        {
            _transport = await result.Content.ReadFromJsonAsync<TokenTransport>(token)!;
            _client.DefaultRequestHeaders.Authorization = new("bearer", _transport.Value.Token);
            _ = SecureStorage.Default.SetAsync(KeyStorageHelper.AuthKey, JsonSerializer.Serialize(_transport));
            // _config.CurrentMode = StorageMode.Full;
            AuthenticationChange.Invoke(AuthenticationState.Connected);
            return Result.Success();
        }
        return result.StatusCode switch
        {
            HttpStatusCode.BadRequest => Result.BadRequest(),
            HttpStatusCode.NotFound => Result.NoFound(),
            _ => Result.TeaBreak()
        };
    }
    /// <summary>
    /// Cierra sesión y limpia los datos de autenticación almacenados.
    /// </summary>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Un resultado que indica éxito o error de la operación.</returns>
    public async Task<Result> SignOutAsync(CancellationToken token = default)
    {
        var result = await _client.GetAsync("auth/sign-out", token);
        if (result.IsSuccessStatusCode)
        {
            _transport = null;
            SecureStorage.Default.Remove(KeyStorageHelper.AuthKey);
            _client.DefaultRequestHeaders.Authorization = null;
            AuthenticationChange.Invoke(AuthenticationState.DisConnected);
            return Result.Success();
        }
        else if (result.StatusCode == HttpStatusCode.Unauthorized)
        {
            SecureStorage.Default.Remove(KeyStorageHelper.AuthKey);
            _client.DefaultRequestHeaders.Authorization = null;
            AuthenticationChange.Invoke(AuthenticationState.DisConnected);
            return Result.Success();
        }
        return Result.NoFound();
    }
    /// <summary>
    /// Registra un nuevo usuario.
    /// </summary>
    /// <param name="request">Solicitud de registro con correo electrónico, nombre de usuario y contraseña.</param>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Un resultado que indica éxito o error de la operación.</returns>
    public async Task<Result> SignUpAsync(SignUpRequest request, CancellationToken token = default)
    {
        var result = await _client.PostAsJsonAsync("auth/sign-up", request, token);
        if (result.IsSuccessStatusCode)
        {
            _transport = await result.Content.ReadFromJsonAsync<TokenTransport>(token)!;
            _client.DefaultRequestHeaders.Authorization = new("bearer", _transport.Value.Token);
            _ = SecureStorage.Default.SetAsync(KeyStorageHelper.AuthKey, JsonSerializer.Serialize(_transport));
            // _config.CurrentMode = StorageMode.Full;
            AuthenticationChange.Invoke(AuthenticationState.Connected);
            return Result.Success();
        }
        return result.StatusCode switch
        {
            HttpStatusCode.Conflict => Result.Conflict(),
            HttpStatusCode.BadRequest => Result.BadRequest(),
            HttpStatusCode.NotFound => Result.NoFound(),
            _ => Result.TeaBreak()
        };
    }
    /// <summary>
    /// Renueva el token de acceso si ha expirado, verificando el token de renovación.
    /// </summary>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Un resultado que indica éxito o error de la operación de renovación.</returns>
    public async Task<Result> SignRefreshAsync(CancellationToken token = default)
    {
        if (_transport is null) return Result.NoContent();
        var currentTime = DateTimeOffset.Now;
        if (_transport.Value.Expiration > currentTime)
            return Result.Success();
        else if (_transport.Value.RefreshExpiration < currentTime)
            return Result.UnAuthorized();
        _client.DefaultRequestHeaders.Authorization = new("bearer", _transport.Value.RefreshToken);
        var result = await _client.GetAsync("auth/refresh", token);
        if (result.IsSuccessStatusCode)
        {
            _transport = await result.Content.ReadFromJsonAsync<TokenTransport>(token);
            AuthenticationChange.Invoke(AuthenticationState.Connected);
            return Result.Success();
        }
        _transport = null;
        AuthenticationChange.Invoke(AuthenticationState.DisConnected);
        return Result.NoFound();
    }
    /// <summary>
    /// Carga los datos de autenticación almacenados en el almacenamiento seguro del dispositivo.
    /// </summary>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    private async Task InitAsync()
    {
        var data = await SecureStorage.Default.GetAsync(KeyStorageHelper.AuthKey);
        if (string.IsNullOrWhiteSpace(data)) return;
        _transport = JsonSerializer.Deserialize<TokenTransport>(data);
        _client.DefaultRequestHeaders.Authorization = new("bearer", _transport.Value.Token);
        AuthenticationChange.Invoke(AuthenticationState.Connected);
    }
    /// <summary>
    /// Libera los recursos gestionados por esta instancia.
    /// </summary>
    /// <param name="disposing">Indica si se debe liberar recursos administrados.</param>
    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _transport = null;
            }
            _disposedValue = true;
        }
    }
    /// <summary>
    /// Libera todos los recursos asociados a esta instancia.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    /// <summary>
    /// Obtiene o establece el token de transporte actual.
    /// </summary>
    private TokenTransport? _transport { get; set; }
    /// <summary>
    /// Evento que se dispara cuando el estado de autenticación cambia.
    /// </summary>
    public Action<AuthenticationState> AuthenticationChange { get; set; }
    /// <summary>
    /// Cliente HTTP para comunicarse con el servidor de autenticación.
    /// </summary>
    private readonly HttpClient _client;
    /// <summary>
    /// Indica si los recursos han sido liberados.
    /// </summary>
    private bool _disposedValue;
}