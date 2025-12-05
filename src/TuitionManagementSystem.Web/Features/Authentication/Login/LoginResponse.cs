namespace TuitionManagementSystem.Web.Auth.Login;

public record LoginResponse(LoginResponseStatus Status)
{
}

public enum LoginResponseStatus
{
    Success,
    TwoFactorRequired
}
