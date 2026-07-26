using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class GetAllPharmacyCompaniesHandler
    : IRequestHandler<GetAllPharmacyCompaniesQuery,
        Result<PaginatedResult<PharmacyCompanyDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetAllPharmacyCompaniesHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<PaginatedResult<PharmacyCompanyDto>>> Handle(
        GetAllPharmacyCompaniesQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

        var query = _uow.Repository<PharmacyCompany>()
            .Query()
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var kw = request.Keyword.Trim().ToLower();
            query = query.Where(x =>
                x.Name.ToLower().Contains(kw) ||
                x.Email.ToLower().Contains(kw));
        }

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var data = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new PharmacyCompanyDto
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Phone = x.Phone,
                Address = x.Address,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<PharmacyCompanyDto>>.Success(
            new PaginatedResult<PharmacyCompanyDto>(
                data, totalCount, pageNumber, pageSize),
            "Companies retrieved successfully");
    }
}