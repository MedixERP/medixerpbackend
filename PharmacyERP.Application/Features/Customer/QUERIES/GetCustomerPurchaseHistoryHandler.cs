using MediatR;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;

public class GetCustomerPurchaseHistoryHandler
    : IRequestHandler<
        GetCustomerPurchaseHistoryQuery,
        Result<CustomerPurchaseHistoryDto>>
{
    private readonly IUnitOfWork _uow;

    public GetCustomerPurchaseHistoryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<CustomerPurchaseHistoryDto>> Handle(
        GetCustomerPurchaseHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var customer =
            await _uow.Customers
                .GetCustomerWithInvoicesAsync(
                    request.CustomerId);

        if (customer == null)
            return Result<CustomerPurchaseHistoryDto>
                .Failure("Customer not found", 404);

        var data = new CustomerPurchaseHistoryDto
        {
            CustomerId = customer.Id,
            CustomerName = customer.FullName,

            Invoices = customer.Invoices
                .Select(i => new InvoiceHistoryDto
                {
                    InvoiceNumber = i.InvoiceNumber,
                    FinalAmount = i.FinalAmount,
                    CreatedAt = i.CreatedAt
                })
                .ToList()
        };

        return Result<CustomerPurchaseHistoryDto>.Success(
            data,
            "Purchase history retrieved successfully");
    }
}
