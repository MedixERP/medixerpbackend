using MediatR;
using PharmacyERP.Application.Common.Models;

public class UpdateCategoryCommand : IRequest<Result<string>>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
}