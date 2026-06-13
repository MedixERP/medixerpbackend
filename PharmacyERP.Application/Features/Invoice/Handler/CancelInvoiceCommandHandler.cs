using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class CancelInvoicesCommandHandler
    : IRequestHandler<
        CancelInvoiceCommand,
        Result<string>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public CancelInvoicesCommandHandler(
        IUnitOfWork uow,
        IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    public async Task<Result<string>> Handle(CancelInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _uow.Repository<Invoice>()
            .Query()
            .Include(x => x.InvoiceItems)
            .FirstOrDefaultAsync(x => x.Id == request.InvoiceId);

        if (invoice == null)
            return Result<string>.Failure("Not found", 404);

        if (invoice.IsCancelled)
            return Result<string>.Failure("Already cancelled", 400);

        using var tx = await _uow.BeginTransactionAsync();

        try
        {
            foreach (var item in invoice.InvoiceItems)
            {
                var batch = await _uow.ProductBatches.GetByIdAsync(item.BatchId);

                if (batch != null)
                    batch.Quantity += item.Quantity;
            }

            invoice.IsCancelled = true;

            await _uow.SaveChangesAsync(cancellationToken);

            await tx.CommitAsync();

            return Result<string>.Success("Cancelled", "Done");
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}