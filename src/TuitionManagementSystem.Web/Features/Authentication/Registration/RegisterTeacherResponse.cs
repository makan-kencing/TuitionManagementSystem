namespace TuitionManagementSystem.Web.Features.Authentication.Registration;

public sealed class RegisterTeacherResponse
{
    public int Id { get; init; }
    public int AccountId { get; init; }
    public string Message { get; init; } = string.Empty;
    public bool IsSuccess { get; init; } = true;
}
