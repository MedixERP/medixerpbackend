using MediatR;
using PharmacyERP.Application.Common.Models;

public class AddCategoryCommand : IRequest<Result<int>>
{
    public string Name { get; set; }
    public string? Description { get; set; }
}