namespace TuitionManagementSystem.Web.Features.Homework.StudentHomework;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;

public sealed class GetStudentHomeworkRequestHandler(ApplicationDbContext db)
    : IRequestHandler<GetStudentHomeworkRequest, Result<GetStudentHomeworkResponse>>
{
    public async Task<Result<GetStudentHomeworkResponse>> Handle(GetStudentHomeworkRequest request,
        CancellationToken cancellationToken)
    {

        var user=await db.Accounts
            .Where(a=>a.User.Id==request.UserId)
            .Select(a=>a.DisplayName)
            .FirstOrDefaultAsync(cancellationToken);

        var enrollInfos = await db.Enrollments
            .Where(e => e.StudentId == request.UserId)
            .Select(e => new StudentEnrollInfo
            {

                CourseId = e.CourseId,
                EnrollDate = e.EnrolledAt.Date
            })
            .ToListAsync(cancellationToken);

        var courseIds = enrollInfos.Select(e => e.CourseId).ToList();

        var coursesWithTeachers = await db.CourseTeachers
            .Where(ct => courseIds.Contains(ct.CourseId))
            .Select(ct => new StudentCoursesInfo
            {
                SubjectName =ct.Course.Subject.Name,
                CourseName = ct.Course.Name,
                CourseSections = new List<StudentCourseSection>(),
                TeacherName = ct.Teacher.Account.Username,
                TeacherId = ct.TeacherId

            })
            .ToListAsync(cancellationToken);


        var response = new GetStudentHomeworkResponse
        {
            Name = user,
            EnrollInfos = enrollInfos,
            CourseInfos = coursesWithTeachers
        };

        return Result<GetStudentHomeworkResponse>.Success(response);
    }
}

