namespace TuitionManagementSystem.Web.Features.Family.ViewFamily;

using Ardalis.Result;
using MediatR;

public class ViewFamilyQueryHandler : IRequestHandler<ViewFamilyQuery, Result<ViewFamilyResponse>>
{
    public Task<Result<ViewFamilyResponse>> Handle(ViewFamilyQuery request, CancellationToken cancellationToken)
    {
        // TODO
        throw new NotImplementedException();
    }
}
