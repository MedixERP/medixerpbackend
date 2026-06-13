using MediatR;

public class ExportLowStockProductsQuery : IRequest<byte[]>
{
    public string Format { get; set; }
}