namespace TuitionManagementSystem.Web.Features.Family.ViewChild;

using Ardalis.Result;
using MediatR;

public class ViewChildQueryHandler : IRequestHandler<ViewChildQuery, Result<ViewChildResponse>>
{
    public async Task<Result<ViewChildResponse>> Handle(ViewChildQuery request, CancellationToken cancellationToken)
    {
        // TODO
        throw new NotImplementedException();
    }
}
