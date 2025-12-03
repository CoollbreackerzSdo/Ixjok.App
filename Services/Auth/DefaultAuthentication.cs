using System.Net;
using System.Net.Http.Json;

namespace Ixjok.Services.Auth;

public sealed class DefaultAuthentication : IBearerAuthentication
{
    public DefaultAuthentication(HttpClient client)
    {
        _client = client;
        if (SecureStorage.Default.GetAsync(KeyStorageHelper.AuthKey).GetAwaiter().GetResult() is string value)
            Token = value;
    }
    public Task<bool> RequestAuthenticationStateAsync(CancellationToken token = default) => Task.FromResult(!string.IsNullOrWhiteSpace(Token));
    public async Task<Result> SignInAsync(SignInRequest request, CancellationToken token = default)
    {
        var result = await _client.PostAsJsonAsync("auth/sign-in", request, token);
        if (result.IsSuccessStatusCode)
        {
            Token = await result.Content.ReadFromJsonAsync<string>(token) ?? string.Empty;
            await SecureStorage.Default.SetAsync(KeyStorageHelper.AuthKey, Token);
            return Result.Success();
        }
        return result.StatusCode switch
        {
            HttpStatusCode.BadRequest => Result.BadRequest(),
            HttpStatusCode.NotFound => Result.NoFound(),
            _ => Result.TeaBreak()
        };
    }
    public Task<Result> SignOutAsync(CancellationToken token = default)
    {
        Token = string.Empty;
        SecureStorage.Default.Remove(KeyStorageHelper.AuthKey);
        return Task.FromResult(Result.Success());
    }
    public async Task<Result> SignUpAsync(SignUpRequest request, CancellationToken token = default)
    {
        var result = await _client.PostAsJsonAsync("auth/sign-up", request, token);
        if (result.IsSuccessStatusCode)
        {
            Token = await result.Content.ReadFromJsonAsync<string>(token) ?? string.Empty;
            await SecureStorage.Default.SetAsync(KeyStorageHelper.AuthKey, Token);
            return Result.Success();
        }
        return result.StatusCode switch
        {
            HttpStatusCode.BadRequest => Result.BadRequest(),
            HttpStatusCode.NotFound => Result.NoFound(),
            _ => Result.TeaBreak()
        };
    }
    public string Token { get; private set; } = string.Empty;
    private readonly HttpClient _client;
}