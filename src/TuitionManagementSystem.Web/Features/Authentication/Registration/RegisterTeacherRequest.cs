namespace TuitionManagementSystem.Web.Features.Authentication.Registration;

using Ardalis.Result;
using MediatR;

public record RegisterTeacherRequest : IRequest<Result<RegisterTeacherResponse>>
{
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
}
