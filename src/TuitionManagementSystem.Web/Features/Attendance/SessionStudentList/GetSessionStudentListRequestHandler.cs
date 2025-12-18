namespace TuitionManagementSystem.Web.Features.Attendance.SessionStudentList;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class GetSessionStudentListRequestHandler(ApplicationDbContext db)
    : IRequestHandler<GetSessionStudentListRequest, Result<GetSessionStudentListResponse>>
{
    public async Task<Result<GetSessionStudentListResponse>> Handle(
        GetSessionStudentListRequest request,
        CancellationToken cancellationToken)
    {
        var session = await db.Sessions
            .Where(s => s.Id == request.SessionId)
            .Select(s => new GetSessionStudentListResponse
            {
                SessionId = s.Id,
                IsCodeGenerated = s.AttendanceCode != null,
                StartDate = s.StartAt,
                EndDate = s.EndAt,
                CreatedOn = DateTime.UtcNow,
                StudentList = s.Course.Enrollments.Join(
                    s.Attendances.DefaultIfEmpty(),
                    e => e.Student.Id,
                    a => a!.Student.Id,
                    (e, a) => new StudentInfo
                    {
                        StudentId = e.Student.Id,
                        DisplayName = e.Student.Account.DisplayName,
                        AttendanceId = a == null ? null : a.Id
                    }
                ).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (session is null)
        {
            return Result.NotFound();
        }

        return Result.Success(session);
    }
}
