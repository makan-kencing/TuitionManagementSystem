namespace TuitionManagementSystem.Web.Features.Authentication.Registration;

public sealed class RegisterParentResponse
{
    public int Id { get; init; }
    public int AccountId { get; init; }
    public string Message { get; init; } = string.Empty;
    public bool IsSuccess { get; init; } = true; // add success flag
}
