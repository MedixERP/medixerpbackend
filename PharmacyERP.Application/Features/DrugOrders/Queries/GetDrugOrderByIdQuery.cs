using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetDrugOrderByIdQuery : IRequest<Result<DrugOrderDto>>
{
    public int Id { get; set; }
}