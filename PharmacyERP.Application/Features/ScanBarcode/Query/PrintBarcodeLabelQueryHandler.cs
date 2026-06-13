using MediatR;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;

public class PrintBarcodeLabelQueryHandler
    : IRequestHandler<PrintBarcodeLabelQuery, Result<byte[]>>
{
    private readonly IUnitOfWork _uow;

    public PrintBarcodeLabelQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<byte[]>> Handle(
       PrintBarcodeLabelQuery request,
       CancellationToken cancellationToken)
    {
        var product = await _uow.Products.GetByIdAsync(request.ProductId);

        if (product == null)
            return Result<byte[]>.Failure("Product not found", 404);

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A7);
                page.Margin(10);

                page.Content().Column(col =>
                {
                    col.Item().Text(product.Name)
                        .Bold()
                        .FontSize(14);

                    col.Item().Text(
                        $"Price: {product.SalePrice:0.00}");

                    if (product.BarcodeImage != null && product.BarcodeImage.Length > 0)
                    {
                        col.Item().Image(product.BarcodeImage);
                    }

                    if (product.QrCodeImage != null && product.QrCodeImage.Length > 0)
                    {
                        col.Item().Image(product.QrCodeImage);
                    }

                    col.Item().Text(product.Barcode)
                        .FontSize(10)
                        .SemiBold();
                });
            });
        }).GeneratePdf();

        return Result<byte[]>.Success(pdf);
    }
}