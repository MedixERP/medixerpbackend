using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class GetAllCustomersHandler
    : IRequestHandler<GetAllCustomersQuery, Result<PaginatedResult<CustomerDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetAllCustomersHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<PaginatedResult<CustomerDto>>> Handle(
        GetAllCustomersQuery request,
        CancellationToken cancellationToken)
    {
       
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

      
        var query = _uow.Customers
            .Query()
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

       
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();

            query = query.Where(x =>
                x.FullName.Contains(keyword) ||
                x.Phone.Contains(keyword));
        }

      
        if (request.IsVip.HasValue)
        {
            query = query.Where(x => x.IsVip == request.IsVip.Value);
        }

        
        var totalCount = await query.CountAsync(cancellationToken);

       
        var customers = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new CustomerDto
            {
                Id = x.Id,
                FullName = x.FullName,
                Phone = x.Phone,
                IsVip = x.IsVip,
                CreditLimit = x.CreditLimit
            })
            .ToListAsync(cancellationToken);

        
        var paginatedResult = new PaginatedResult<CustomerDto>(
            customers,
            totalCount,
            pageNumber,
            pageSize
        );

        return Result<PaginatedResult<CustomerDto>>
            .Success(paginatedResult, "Customers retrieved successfully");
    }
}