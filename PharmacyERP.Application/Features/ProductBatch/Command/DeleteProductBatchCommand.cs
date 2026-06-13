using MediatR;
using PharmacyERP.Application.Common.Models;

public class DeleteProductBatchCommand : IRequest<Result<MediatR.Unit>>
{
    public int Id { get; set; }
}