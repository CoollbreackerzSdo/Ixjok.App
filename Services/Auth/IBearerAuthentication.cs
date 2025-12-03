namespace Ixjok.Services.Auth;

public interface IBearerAuthentication : IAuthentication
{
    string Token { get; }
}