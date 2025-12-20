namespace TuitionManagementSystem.Web.Features.Homework.GetSubmissionFile;

using Ardalis.Result;
using File.UploadFiles;
using GetAssignmentDetail;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetSubmissionFileRequestHandler(ApplicationDbContext db)
    : IRequestHandler< GetSubmissionFileRequest, Result< GetSubmissionFileResponse>>
{
    public async Task<Result<GetSubmissionFileResponse>> Handle(GetSubmissionFileRequest request,
        CancellationToken cancellationToken)
    {
        var file = await db.Submissions
            .Where(s => s.Id == request.SubmissionId)
            .Select(s=> new GetSubmissionFileResponse
            {
               Name = s.Student.Account.Name,
               Id =  s.Id,
               SubmissionFiles =new UploadFilesResponse(s.Attachments.Select(
                   sf=> new UploadedFile
                       (
                       sf.Id,
                       sf.File.FileName,
                       sf.File.MimeType,
                       sf.File.Uri
                   )).ToList())
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (file==null)
        {
            return Result.NotFound();
        }

        return Result.Success(file);


    }

}
