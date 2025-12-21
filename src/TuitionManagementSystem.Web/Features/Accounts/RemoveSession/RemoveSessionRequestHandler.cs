namespace TuitionManagementSystem.Web.Features.Accounts.RemoveSession;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class RemoveSessionRequestHandler(ApplicationDbContext db)
    : IRequestHandler<RemoveSessionRequest, Result<RemoveSessionResponse>>
{
    public async Task<Result<RemoveSessionResponse>> Handle(RemoveSessionRequest request, CancellationToken cancellationToken)
    {
        var session = await db.AccountSessions
            .FirstOrDefaultAsync(s => s.SessionId == request.SessionId, cancellationToken);

        if (session == null)
        {
            return Result<RemoveSessionResponse>.NotFound();
        }

        db.AccountSessions.Remove(session);
        await db.SaveChangesAsync(cancellationToken);

        return Result<RemoveSessionResponse>.Success(new RemoveSessionResponse("Session removed successfully."));
    }
}
