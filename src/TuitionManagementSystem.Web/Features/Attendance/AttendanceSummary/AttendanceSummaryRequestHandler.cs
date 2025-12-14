namespace TuitionManagementSystem.Web.Features.Attendance.AttendanceSummary;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class AttendanceSummaryRequestHandler(
    ApplicationDbContext db,
    IHttpContextAccessor httpContextAccessor)
{
    public async Task<Result<AttendanceSummaryViewModel>> Handle(AttendanceSummaryRequest request,
        CancellationToken cancellationToken)
    {
        var enrollments = await db.Enrollments
            .Where(e=>e.Student.Id==request.StudentId)
            .ToListAsync(cancellationToken);

        var student=await db.Students
            .Where(s => s.Id == request.StudentId)
            .FirstOrDefaultAsync(cancellationToken);

        var totalCourses = enrollments.Count;
        var currentHavingCourse = enrollments;

        var courseSummaries = new List<CourseSummaryViewModel>();

        foreach (var enrollment in enrollments)
        {
           var course = enrollment.Course;

           var attendanceStats = await this.CalculateCourseAttendance(request.StudentId, course.Id);

           var courseSummary = new CourseSummaryViewModel
           {
               CourseId = course.Id,
               CourseName = course.Name,
               TotalSessions = attendanceStats.TotalSessions,
               AttendedSessions = attendanceStats.AttendedSessions,
               AttendanceRate = attendanceStats.AttendanceRate
           };

           courseSummaries.Add(courseSummary);

        }
        return new AttendanceSummaryViewModel
                    {
                     StudentId   = request.StudentId,
                     StudentName = student.Account.Username,

                     Courses = courseSummaries
                    };

    }

    private async Task<CourseAttendanceStats> CalculateCourseAttendance(int studentId,int courseId)
    {
        var attendances = await db.Attendances
            .Where(a => a.Student.Id == studentId &&
                        a.Session.Course.Id == courseId)
            .ToListAsync();

        var sessions=await db.Sessions
            .Where(s => s.Course.Id == courseId)
            .ToListAsync();

        var totalSessions = sessions.Count;
        var attendedSessions = attendances.Count;
        var attendanceRate = totalSessions > 0
            ? (decimal)attendedSessions / totalSessions * 100
            : 0;

        return new CourseAttendanceStats
        {
            TotalSessions = totalSessions,
            AttendedSessions = attendedSessions,
            AttendanceRate = attendanceRate
        };
    }

    private class CourseAttendanceStats
    {
        public int TotalSessions { get; set; }
        public int AttendedSessions { get; set; }
        public decimal AttendanceRate { get; set; }
    }

}
