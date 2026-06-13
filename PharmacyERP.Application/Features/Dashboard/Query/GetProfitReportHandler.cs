using MediatR;

public class GetProfitReportHandler
    : IRequestHandler<GetProfitReportQuery, ProfitReportDto>
{
    private readonly IUnitOfWork _uow;

    public GetProfitReportHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<ProfitReportDto> Handle(
        GetProfitReportQuery request,
        CancellationToken cancellationToken)
    {
        return await _uow
            .Dashboard
            .GetProfitReportAsync();
    }
}