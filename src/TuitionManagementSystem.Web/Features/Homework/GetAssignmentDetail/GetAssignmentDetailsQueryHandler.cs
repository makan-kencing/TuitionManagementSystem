namespace TuitionManagementSystem.Web.Features.Homework.GetAssignmentDetail;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetAssignmentDetailsQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetAssignmentDetailsQuery, Result<GetAssignmentDetailsResponse>>
{
    public async Task<Result<GetAssignmentDetailsResponse>> Handle(GetAssignmentDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var assignmentDetails = await db.Assignments
            .Where(a => a.Id == request.AssignmentId)
            .Select(a => new GetAssignmentDetailsResponse
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                DueAt = a.DueAt,
                Assigned = a.Course.Enrollments
                    .Join(
                        a.Submissions.DefaultIfEmpty(),
                        e => e.Student.Id,
                        s => s!.Student.Id,
                        (e, s) => new { Enrollment = e, Submission = s }
                    )
                    .Select(g => new StudentHomework
                    {
                        StudentName = g.Enrollment.Student.Account.DisplayName,
                        Submission = g.Submission == null
                            ? null
                            : new AssignmentSubmission
                            {
                                Id = g.Submission.Id,
                                Content = g.Submission.Content,
                                Grade = g.Submission.Grade,
                                SubmittedAt = g.Submission.SubmittedAt,
                                Files = g.Submission.Attachments.Select(sf =>
                                    new AssignmentSubmissionFile
                                    {
                                        FileName = sf.File.FileName,
                                        MimeType = sf.File.MimeType,
                                        MappedPath = sf.File.Uri
                                    }).ToList()
                            }
                    })
                    .ToList(),
                TotalStudents = a.Course.Enrollments.Count(),
                SubmittedCount = a.Submissions.Count(),
                AverageSubmissionRate = (double)a.Submissions.Count() / a.Course.Enrollments.Count() *100
            })
            .FirstOrDefaultAsync(cancellationToken);


        if (assignmentDetails is null)
        {
            return Result.NotFound();
        }

        return Result.Success(assignmentDetails);
    }
}
