using MediatR;
using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Domain.Enums;
using System.Security.Claims;

public class AddExpenseCommandHandler
    : IRequestHandler<AddExpenseCommand, Result<int>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public AddExpenseCommandHandler(
        IUnitOfWork uow,
        IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    public async Task<Result<int>> Handle(
        AddExpenseCommand request,
        CancellationToken cancellationToken)
    {
        var userIdClaim = _http.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userId = userIdClaim != null ? int.Parse(userIdClaim) : 0;

        var expense = new Expense
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Amount = request.Amount,
            PaidAt = request.PaidAt,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<Expense>().AddAsync(expense);

        var cashbox = new CashboxTransaction
        {
            Type = CashboxTransactionType.Out,
            Source = CashboxSource.Expense,
            Amount = request.Amount,
            ReferenceType = "Expense",
            Description = request.Title,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<CashboxTransaction>().AddAsync(cashbox);
        await _uow.SaveChangesAsync(cancellationToken);

        cashbox.ReferenceId = expense.Id;
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(expense.Id, "Expense added successfully");
    }
}