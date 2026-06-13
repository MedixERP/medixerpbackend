using MediatR;
using PharmacyERP.Application.Common.Models;

public class DeleteProductCommand : IRequest<Result<string>>
{
    public int Id { get; set; }
}