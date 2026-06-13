using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class GetCustomerByIdQueryHandler
    : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
{
    private readonly IUnitOfWork _uow;

    public GetCustomerByIdQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<CustomerDto>> Handle(
        GetCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        var customer = await _uow.Repository<Customer>()
            .Query()
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.Id == request.Id)
            .Select(x => new CustomerDto
            {
                Id = x.Id,
                FullName = x.FullName,
                Phone = x.Phone,
                IsVip = x.IsVip,
                CreditLimit = x.CreditLimit
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (customer is null)
        {
            return Result<CustomerDto>.Failure(
                "Customer not found",
                404);
        }

        return Result<CustomerDto>.Success(
            customer,
            "Customer retrieved successfully");
    }
}