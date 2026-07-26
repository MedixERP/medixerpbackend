using MediatR;
using PharmacyERP.Application.Common.Models;

public class AssignSupplierCommand : IRequest<Result<string>>
{
    public int OrderId { get; set; }
    public string SupplierName { get; set; }
    public string SupplierPhone { get; set; }
}