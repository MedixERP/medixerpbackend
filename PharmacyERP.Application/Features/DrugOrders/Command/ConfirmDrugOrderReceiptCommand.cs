using MediatR;
using PharmacyERP.Application.Common.Models;

public class ConfirmDrugOrderReceiptCommand : IRequest<Result<string>>
{
    public int OrderId { get; set; }
}