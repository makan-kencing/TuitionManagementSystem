namespace TuitionManagementSystem.Web.Features.Accounts;

using MediatR;

public record ChangePasswordRequest : IRequest<ChangePasswordResponse>
{
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
}
