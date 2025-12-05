namespace TuitionManagementSystem.Web.Auth.Logout;

using Ardalis.Result;
using MediatR;

public record LogoutRequest : IRequest<Result>;
