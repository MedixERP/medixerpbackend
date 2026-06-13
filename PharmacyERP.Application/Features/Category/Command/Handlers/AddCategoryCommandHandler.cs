using MediatR;
using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using System.Security.Claims;

public class AddCategoryCommandHandler
    : IRequestHandler<AddCategoryCommand, Result<int>>
{
    private readonly IUnitOfWork _uow;

    public AddCategoryCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<int>> Handle(
        AddCategoryCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<int>.Failure("Name is required", 400);

        var exists = await _uow.Categories.IsNameExistsAsync(request.Name);

        if (exists)
            return Result<int>.Failure("Category already exists", 400);

        var category = new Category
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _uow.Categories.AddAsync(category);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(category.Id, "Category created successfully");
    }
}