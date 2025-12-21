namespace TuitionManagementSystem.Web.Features.Homework.GetAnnouncementDetail;

using Ardalis.Result;
using MediatR;

public record GetAnnouncementDetailRequest(int AssignmentId,int StudentId)
    : IRequest<Result<GetAnnouncementDetailResponse>>;
