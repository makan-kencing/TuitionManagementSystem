namespace TuitionManagementSystem.Web.Features.Homework.GetAnnouncementDetail;

using Ardalis.Result;
using File.UploadFiles;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetAnnouncementDetailRequestHandler(ApplicationDbContext db)
    : IRequestHandler<GetAnnouncementDetailRequest, Result<GetAnnouncementDetailResponse>>
{
    public async Task<Result<GetAnnouncementDetailResponse>> Handle(GetAnnouncementDetailRequest request,
        CancellationToken cancellationToken)
    {
        var assignmentDetails = await db.Assignments
            .Where(a => a.Id == request.AssignmentId)
            .Select(a => new GetAnnouncementDetailResponse
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                DueAt = a.DueAt,
                AnnouncementFiles = new UploadFilesResponse(a.Attachments.Select(af => new UploadedFile
                (
                    af.Id,
                    af.File.FileName,
                    af.File.MimeType,
                    af.File.Uri
                )).ToList()),
                Assigned = a.Submissions
                    .Where(s => s.Student.Id == request.StudentId)
                    .Select(s => new StudentHomework
                    {
                        Submission = new AssignmentSubmission
                        {
                            Id = s.Id,
                            Content = s.Content,
                            Grade = s.Grade,
                            SubmittedAt = s.SubmittedAt,
                            Files = s.Attachments
                                .Select(sf => new AssignmentSubmissionFile
                                {
                                    Id =  sf.Id,
                                    FileName = sf.File.FileName,
                                    MimeType = sf.File.MimeType,
                                    MappedPath = sf.File.Uri
                                }).ToList()
                        }
                    }).FirstOrDefault()
            }).FirstOrDefaultAsync(cancellationToken);


        return Result.Success(assignmentDetails);
    }
}
