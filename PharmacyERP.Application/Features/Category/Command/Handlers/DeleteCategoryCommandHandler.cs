using MediatR;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;

public class DeleteCategoryCommandHandler
    : IRequestHandler<DeleteCategoryCommand, Result<string>>
{
    private readonly IUnitOfWork _uow;

    public DeleteCategoryCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
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

        return Result<string>.Success("Deleted", "Category deleted successfully");
    }
}