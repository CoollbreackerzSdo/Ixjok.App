using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Ixjok.Tools.Common.Models.Bearer;

namespace Ixjok.Services.Auth;

public sealed partial class JwtHostedAuthenticationHandler : IBearerAuthenticationHandler, IDisposable
{
    public JwtHostedAuthenticationHandler(HttpClient client)
    {
        _client = client;
        AuthenticationChange = (_) => { };
        InitAsync().ConfigureAwait(false);
    }
    public async Task<bool> AuthenticationStateAsync(CancellationToken token = default)
    {
        if (_transport is null) return false;
        await SignRefreshAsync(token);
        return _transport is not null;
    }
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
    private async Task InitAsync()
    {
        var data = await SecureStorage.Default.GetAsync(KeyStorageHelper.AuthKey);
        if (string.IsNullOrWhiteSpace(data)) return;
        _transport = JsonSerializer.Deserialize<TokenTransport>(data);
        _client.DefaultRequestHeaders.Authorization = new("bearer", _transport.Value.Token);
        AuthenticationChange.Invoke(AuthenticationState.Connected);
    }
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
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    private TokenTransport? _transport { get; set; }
    public Action<AuthenticationState> AuthenticationChange { get; set; }
    private readonly HttpClient _client;
    private bool _disposedValue;
}