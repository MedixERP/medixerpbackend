using Application.Common.Interfaces;
using MediatR;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;

public class UpdateCategoryCommandHandler
    : IRequestHandler<UpdateCategoryCommand, Result<string>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICacheService _cache;

    public UpdateCategoryCommandHandler(IUnitOfWork uow, ICacheService cache)
    {
        _uow = uow;
        _cache = cache;
    }

    public async Task<Result<string>> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _uow.Categories.GetByIdAsync(request.Id);
        if (category == null || category.IsDeleted)
            return Result<string>.Failure("Category not found", 404);

        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<string>.Failure("Name is required", 400);

        var exists = await _uow.Categories.IsNameExistsAsync(request.Name);
        if (exists && !category.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase))
            return Result<string>.Failure("Category name already exists", 400);

        category.Name = request.Name.Trim();
        category.Description = request.Description?.Trim();
        category.UpdatedAt = DateTime.UtcNow;

        _uow.Categories.Update(category);
        await _uow.SaveChangesAsync(cancellationToken);

        await _cache.RemoveByPatternAsync("categories:*", cancellationToken);

        return Result<string>.Success("Updated", "Category updated successfully");
    }
}