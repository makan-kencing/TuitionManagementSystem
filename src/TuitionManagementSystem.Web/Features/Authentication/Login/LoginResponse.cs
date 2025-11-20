namespace TuitionManagementSystem.Web.Features.Authentication.Login;

public record LoginResponse(LoginResponseStatus? ErrorStatus = null)
{
    public bool IsError => this.ErrorStatus != null;
}

public enum LoginResponseStatus
{
    Invalid,
    TwoFactorRequired
}
