using MediatR;

public class ExportPurchaseOrdersQuery : IRequest<byte[]>
{
    public string Format { get; set; }
}