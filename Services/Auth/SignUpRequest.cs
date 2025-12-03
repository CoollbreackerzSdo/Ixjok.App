namespace Ixjok.Services.Auth;

public record struct SignUpRequest(string UserName, string Password)
{
    public string UserName { get; init; } = UserName;
    public string Password { get; init; } = Password;
}