namespace Ixjok.Services.Auth;

public interface IAuthentication
{
    Task<bool> RequestAuthenticationStateAsync(CancellationToken token = default);
    Task<Result> SignOutAsync(CancellationToken token = default);
    Task<Result> SignInAsync(SignInRequest request, CancellationToken token = default);
    Task<Result> SignUpAsync(SignUpRequest request, CancellationToken token = default);
}