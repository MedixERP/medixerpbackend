using Application.Common.Interfaces;
using MediatR;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;

public class DeleteCategoryCommandHandler
    : IRequestHandler<DeleteCategoryCommand, Result<string>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICacheService _cache;

    public DeleteCategoryCommandHandler(IUnitOfWork uow, ICacheService cache)
    {
        _uow = uow;
        _cache = cache;
    }

    public async Task<Result<string>> Handle(
        DeleteCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _uow.Categories.GetByIdAsync(request.Id);
        if (category == null || category.IsDeleted)
            return Result<string>.Failure("Category not found", 404);

        var hasProducts = await _uow.Products.AnyAsync(
            p => p.CategoryId == request.Id && !p.IsDeleted);
        if (hasProducts)
            return Result<string>.Failure(
                "Cannot delete category because it contains products",
                400);

        category.IsDeleted = true;
        category.UpdatedAt = DateTime.UtcNow;

        _uow.Categories.Update(category);
        await _uow.SaveChangesAsync(cancellationToken);

        await _cache.RemoveByPatternAsync("categories:*", cancellationToken);

        return Result<string>.Success("Deleted", "Category deleted successfully");
    }
}