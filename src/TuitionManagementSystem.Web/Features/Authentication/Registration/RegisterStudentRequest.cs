namespace TuitionManagementSystem.Web.Features.Authentication.Registration;

using Ardalis.Result;
using MediatR;

public record RegisterStudentRequest
    : IRequest<Result<RegisterStudentResponse>>
{
    public string Username { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
}
