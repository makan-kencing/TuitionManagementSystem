namespace TuitionManagementSystem.Web.Features.Accounts;

public sealed class ChangePasswordResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;

    public static ChangePasswordResponse Ok(string message)
        => new() { Success = true, Message = message };

    public static ChangePasswordResponse Fail(string message)
        => new() { Success = false, Message = message };
}
