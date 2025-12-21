namespace TuitionManagementSystem.Web.Features.Dashboard.TeacherDashboard
{
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using TuitionManagementSystem.Web.Infrastructure.Persistence;
    using TuitionManagementSystem.Web.Models.Class;

    public class TeacherDashboardRequestHandler(ApplicationDbContext db)
        : IRequestHandler<TeacherDashboardRequest, TeacherDashboardResponse>
    {
        public async Task<TeacherDashboardResponse> Handle(TeacherDashboardRequest request,
            CancellationToken cancellationToken)
        {
            var todayUtc = DateTime.UtcNow.Date;
            var tomorrowUtc = todayUtc.AddDays(1);

            var teacherCourses = await db.CourseTeachers
                .Where(ct => ct.TeacherId == request.TeacherId)
                .Select(ct => ct.CourseId)
                .ToListAsync(cancellationToken);

            if (!teacherCourses.Any())
            {
                return new TeacherDashboardResponse
                {
                    TotalSessionsToday = 0,
                    ClassesToTeachToday = 0,
                    TotalStudents = 0,
                    HomeworkPending = 0,
                    AttendancePerSession = new Dictionary<string, int> { { "No Sessions", 0 } },
                    SubmissionPerCourse = new Dictionary<string, int> { { "No Courses", 0 } }
                };
            }

            var sessionsToday = await db.Sessions
                .Include(s => s.Course)
                .ThenInclude(c => c.Enrollments)
                .Include(s => s.Attendances)
                .Where(s => teacherCourses.Contains(s.CourseId) &&
                            s.StartAt >= todayUtc &&
                            s.StartAt < tomorrowUtc)
                .ToListAsync(cancellationToken);

            var totalStudentsInCourses = await db.Enrollments
                .Where(e => teacherCourses.Contains(e.CourseId))
                .Select(e => e.StudentId)
                .Distinct()
                .CountAsync(cancellationToken);

            var response = new TeacherDashboardResponse
            {
                TotalSessionsToday = sessionsToday.Count,
                ClassesToTeachToday = sessionsToday.Count,
                TotalStudents = totalStudentsInCourses,
                HomeworkPending = await db.Assignments
                    .Where(a => teacherCourses.Contains(a.CourseId) &&
                                !a.Submissions.Any())
                    .CountAsync(cancellationToken),
                AttendancePerSession = new Dictionary<string, int>(),
                SubmissionPerCourse = new Dictionary<string, int>()
            };

            foreach (var session in sessionsToday)
            {
                var attendanceCount = await db.Attendances
                    .CountAsync(a => a.SessionId == session.Id, cancellationToken);

                response.AttendancePerSession.Add(
                    $"{session.Course.Name} ({session.StartAt:HH:mm})",
                    attendanceCount);
            }

            foreach (var courseId in teacherCourses)
            {
                var course = await db.Courses
                    .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);

                if (course != null)
                {
                    var totalAssignments = await db.Assignments
                        .CountAsync(a => a.CourseId == courseId, cancellationToken);

                    if (totalAssignments > 0)
                    {
                        var submittedCount = await db.Submissions
                            .CountAsync(s => s.Assignment.CourseId == courseId, cancellationToken);

                        var percentage = (int)Math.Round((double)submittedCount / totalAssignments * 100);
                        response.SubmissionPerCourse.Add(course.Name, percentage);
                    }
                    else
                    {
                        response.SubmissionPerCourse.Add(course.Name, 0);
                    }
                }
            }

            if (!response.AttendancePerSession.Any())
                response.AttendancePerSession.Add("No Sessions Today", 0);

            if (!response.SubmissionPerCourse.Any())
                response.SubmissionPerCourse.Add("No Courses", 0);

            return response;
        }
    }
}
