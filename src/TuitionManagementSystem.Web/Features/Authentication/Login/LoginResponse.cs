namespace TuitionManagementSystem.Web.Features.Authentication.Login;

public record LoginResponse(LoginResponseStatus Status)
{
}

public enum LoginResponseStatus
{
    Success,
    TwoFactorRequired
}
