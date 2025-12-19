namespace TuitionManagementSystem.Web.Features.Homework.GetAnnouncementInfo;

using Ardalis.Result;
using MediatR;

public record GetAnnouncementInfoRequest(int CourseId)
    : IRequest<Result<GetAnnouncementInfoResponse>>;
