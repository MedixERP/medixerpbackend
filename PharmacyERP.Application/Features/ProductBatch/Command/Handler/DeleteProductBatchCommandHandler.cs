using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class DeleteProductBatchCommandHandler
    : IRequestHandler<DeleteProductBatchCommand, Result<MediatR.Unit>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public DeleteProductBatchCommandHandler(IUnitOfWork uow, IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    public async Task<Result<MediatR.Unit>> Handle(
        DeleteProductBatchCommand request,
        CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;

        if (user == null || !user.Identity!.IsAuthenticated)
            return Result<MediatR.Unit>.Failure("Unauthorized", 401);

        if (!user.IsInRole("Admin"))
            return Result<MediatR.Unit>.Failure("Only Admin can delete batches", 403);

        var batch = await _uow.ProductBatches.GetByIdAsync(request.Id);

        if (batch == null || batch.IsDeleted)
            return Result<MediatR.Unit>.Failure("Batch not found", 404);

        var usedInInvoices = await _uow.Repository<InvoiceItem>()
       .Query()
       .AsNoTracking()
       .AnyAsync(x =>
           x.BatchId == batch.Id &&
           !x.IsDeleted,
           cancellationToken);

if (usedInInvoices)
        return Result<MediatR.Unit>.Failure(
            "Cannot delete batch because it is used in invoices",
            400);

    if (batch.Quantity > 0)
        return Result<MediatR.Unit>.Failure(
            "Cannot delete batch with remaining stock",
            400);
        batch.IsDeleted = true;
        batch.UpdatedAt = DateTime.UtcNow;

        _uow.ProductBatches.Update(batch);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<MediatR.Unit>.Success(MediatR.Unit.Value);
    }
}