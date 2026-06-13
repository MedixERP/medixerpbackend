using MediatR;

public class GetAlertsHandler
    : IRequestHandler<
        GetAlertsQuery,
        AlertsDto>
{
    private readonly IUnitOfWork _uow;

    public GetAlertsHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<AlertsDto> Handle(
        GetAlertsQuery request,
        CancellationToken cancellationToken)
    {
        return await _uow
            .Dashboard
            .GetAlertsAsync();
    }
}