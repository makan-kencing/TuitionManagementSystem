namespace TuitionManagementSystem.Web.Features.Authentication.Login;

using Ardalis.Result;
using MediatR;

public record LoginRequest(
    string Username,
    string Password,
    bool RememberMe) : IRequest<Result<LoginResponse>>;
