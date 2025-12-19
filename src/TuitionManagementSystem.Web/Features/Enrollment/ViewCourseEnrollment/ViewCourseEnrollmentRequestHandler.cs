namespace TuitionManagementSystem.Web.Features.Enrollment.ViewCourseEnrollment;

using Ardalis.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using Models.Class;

public class ViewCourseEnrollmentsHandler : IRequestHandler<ViewCourseEnrollmentsRequest, Result<List<ViewCourseEnrollmentsResponse>>>
{
    private readonly ApplicationDbContext _context;

    public ViewCourseEnrollmentsHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ViewCourseEnrollmentsResponse>>> Handle(
        ViewCourseEnrollmentsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var course = await _context.Courses
                .Include(c => c.Subject)
                .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

            if (course == null)
                return Result<List<ViewCourseEnrollmentsResponse>>.NotFound("Course not found");

            var enrollments = await this._context.Enrollments
                .Include(e => e.Student).ThenInclude(user => user.Account)
                .Where(e => e.CourseId == request.CourseId && e.Status == Enrollment.EnrollmentStatus.Active)
                .ToListAsync(cancellationToken);

            if (!enrollments.Any())
                return Result<List<ViewCourseEnrollmentsResponse>>.NotFound("No active enrollments found");

            var totalSessions = await _context.Sessions
                .Where(s => s.CourseId == request.CourseId)
                .CountAsync(cancellationToken);

            var response = new List<ViewCourseEnrollmentsResponse>();

            foreach (var enrollment in enrollments)
            {
                var attendedSessions = await _context.Attendances
                    .Include(a => a.Session)
                    .Where(a => a.StudentId == enrollment.StudentId &&
                                a.Session.CourseId == request.CourseId)
                    .CountAsync(cancellationToken);

                decimal attendancePercentage = totalSessions > 0
                    ? Math.Round((attendedSessions * 100m) / totalSessions, 2)
                    : 0m;

                response.Add(new ViewCourseEnrollmentsResponse
                {
                    EnrollmentId = enrollment.Id,
                    StudentId = enrollment.StudentId,
                    StudentName = $"{enrollment.Student.Account.DisplayName}",
                    EnrolledAt = enrollment.EnrolledAt,
                    AttendancePercentage = attendancePercentage,
                    TotalSessions = totalSessions,
                    AttendedSessions = attendedSessions,
                    CourseName = course.Name,
                    SubjectName = course.Subject?.Name ?? "No Subject"
                });
            }

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            return Result<List<ViewCourseEnrollmentsResponse>>.Error(ex.Message);
        }
    }
}

