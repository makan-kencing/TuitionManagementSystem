namespace TuitionManagementSystem.Web.Features.Attendance.AttendanceHistory;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class GetAttendanceHistoryRequestHandler(ApplicationDbContext db)
    : IRequestHandler<GetAttendanceHistoryRequest, Result<GetAttendanceHistoryResponse>>
{
    public async Task<Result<GetAttendanceHistoryResponse>> Handle(
        GetAttendanceHistoryRequest request,
        CancellationToken cancellationToken)
    {

        var enrollmentAt=await  db.Enrollments
            .Where(a => a.CourseId == request.CourseId)
            .Where(a => a.StudentId == request.UserId)
            .Select(a=> a.EnrolledAt)
            .FirstOrDefaultAsync(cancellationToken);

        var response = await db.Courses
            .Where(c => c.Id == request.CourseId)
            .Select(c => new GetAttendanceHistoryResponse
            {
                CourseName = c.Name,
                EnrollmentAt = enrollmentAt,
                Sessions = c.Sessions.Select(s => new SessionAttendanceItem
                    {
                        SessionId = s.Id,
                        StartAt = s.StartAt,
                        EndAt = s.EndAt,
                        IsValid = s.Attendances.Any(a => a.Student.Id == request.UserId)
                    })
                    .OrderBy(s => s.StartAt)
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (response is null)
        {
            Result.NotFound("Course not found");
        }

        return Result.Success(response!);
    }
}
