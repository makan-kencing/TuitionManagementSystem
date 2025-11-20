namespace TuitionManagementSystem.Web.Features.Authentication.Logout;

using Ardalis.Result;
using MediatR;

public record LogoutRequest : IRequest<Result>;
