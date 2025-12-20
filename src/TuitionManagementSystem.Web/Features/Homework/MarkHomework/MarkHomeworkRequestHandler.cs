namespace TuitionManagementSystem.Web.Features.Homework.MarkHomework;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class MarkHomeworkRequestHandler(ApplicationDbContext db)
    : IRequestHandler<MarkHomeworkRequest, Result<MarkHomeworkResponse>>
{
    public async Task<Result<MarkHomeworkResponse>> Handle(MarkHomeworkRequest request,
        CancellationToken cancellationToken)
    {
        await db.Submissions
            .Where(s => s.Id == request.SubmissionId)
            .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(s => s.Grade, request.Grade),
                cancellationToken);

        return Result.Success();
    }
}

