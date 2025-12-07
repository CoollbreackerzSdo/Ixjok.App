namespace Ixjok.Services.Auth;

public interface IBearerAuthenticationHandler : IAuthentication
{
    Task<Result> SignRefreshAsync(CancellationToken token = default);
}