namespace TuitionManagementSystem.Web.Features.Homework.MakeSubmission;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Models.Class.Announcement;

public sealed class MakeSubmissionRequestHandler(ApplicationDbContext db)
    :IRequestHandler<MakeSubmissionRequest,Result<MakeSubmissionResponse>>
{
    public async Task<Result<MakeSubmissionResponse>> Handle(MakeSubmissionRequest request,
        CancellationToken cancellationToken)
    {
        var submission = new Submission
        {
            Content = request.Content,
            AssignmentId = request.AssignmentId,
            StudentId = request.UserId,
            Attachments = request.FileIds
                .Select(id => new SubmissionFile
                {
                    FileId = id
                }).ToList()
        };

        await db.Submissions.AddRangeAsync(submission);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
