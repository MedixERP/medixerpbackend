using MediatR;

public class ExportLowStockQuery : IRequest<byte[]>
{
    public string Format { get; set; }
}