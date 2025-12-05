namespace TuitionManagementSystem.Web.Auth.Login;

using Ardalis.Result;
using MediatR;

public record LoginRequest(
    string Username,
    string Password,
    string? TwoFactorToken,
    bool RememberMe) : IRequest<Result<LoginResponse>>;
