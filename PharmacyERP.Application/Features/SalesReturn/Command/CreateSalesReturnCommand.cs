using MediatR;
using PharmacyERP.Application.Common.Models;

public class CreateSalesReturnCommand : IRequest<Result<int>>
{
    public int InvoiceId { get; set; }
    public string Reason { get; set; }
    public List<CreateSalesReturnItemDto> Items { get; set; }
}

public class CreateSalesReturnItemDto
{
    public int ProductId { get; set; }
    public int BatchId { get; set; }
    public int Quantity { get; set; }
}