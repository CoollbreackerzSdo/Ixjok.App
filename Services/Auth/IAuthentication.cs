namespace Ixjok.Services.Auth;

public interface IAuthentication
{
    public Action<AuthenticationState> AuthenticationChange { get; set; }
    Task<bool> AuthenticationStateAsync(CancellationToken token = default);
    Task<Result> SignOutAsync(CancellationToken token = default);
    Task<Result> SignInAsync(SignInRequest request, CancellationToken token = default);
    Task<Result> SignUpAsync(SignUpRequest request, CancellationToken token = default);
}

public enum AuthenticationState
{
    Connected,
    DisConnected,
    None
}