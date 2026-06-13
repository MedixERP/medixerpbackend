using MediatR;
using PharmacyERP.Application.Common.Models;

public class DeleteCategoryCommand : IRequest<Result<string>>
{
    public int Id { get; set; }
}