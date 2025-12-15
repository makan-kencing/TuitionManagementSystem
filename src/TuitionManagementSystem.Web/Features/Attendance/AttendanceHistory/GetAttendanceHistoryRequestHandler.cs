namespace TuitionManagementSystem.Web.Features.Attendance.AttendanceHistory;

using Ardalis.Result;
using AutoMapper;
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


        var courseName = await db.Courses
            .Where(c => c.Id == request.CourseId)
            .Select(c => c.Name)
            .FirstOrDefaultAsync(cancellationToken);

        if (courseName == null)
        {
            return Result.NotFound("Course not found");
        }

        var list = await db.Sessions
            .Where(s => s.Course.Id == request.CourseId)
            .Select(s => new SessionAttendanceItem
            {
                SessionId = s.Id,
                StartAt = s.StartAt,
                EndAt = s.EndAt,
                IsValid = s.Attendances.Any(a => a.Student.Id ==request.UserId)
            })
            .OrderBy(s => s.StartAt)
            .ToListAsync(cancellationToken);

        return Result.Success(new GetAttendanceHistoryResponse
        {
            Course = courseName,
            Sessions = list
        });
    }


}
