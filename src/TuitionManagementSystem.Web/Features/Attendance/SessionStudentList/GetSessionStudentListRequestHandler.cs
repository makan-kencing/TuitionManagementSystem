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
            .Select(s => new
            {
                s.CourseId,
                s.StartAt,
                s.EndAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (session == null)
            return Result<GetSessionStudentListResponse>.NotFound();

        var enrolledStudents = await db.Enrollments
            .Where(e => e.CourseId == session.CourseId)
            .Select(e => new
            {
                e.StudentId,
                Name = e.Student.Account.DisplayName
            })
            .ToListAsync(cancellationToken);

        var attendanceRecords = await db.Attendances
            .Where(a => a.SessionId == request.SessionId)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;

        var studentList = enrolledStudents.Select(s =>
        {
            var record = attendanceRecords.FirstOrDefault(a => a.StudentId == s.StudentId);

            string status;

            if (record != null)
            {
                status = "Attend";
            }
            else if (now < session.EndAt)
            {
                status = "NotTaken";
            }
            else
            {
                status = "Absent";
            }

            return new StudentInfo
            {
                StudentId = s.StudentId,
                Name = s.Name,
                Status = status
            };
        }).ToList();

        return Result<GetSessionStudentListResponse>.Success(
            new GetSessionStudentListResponse
            {
                StudentList = studentList
            }
        );
    }
}
