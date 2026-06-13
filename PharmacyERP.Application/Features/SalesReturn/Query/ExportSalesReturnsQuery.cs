using MediatR;

public class ExportSalesReturnsQuery : IRequest<byte[]>
{
    public string Format { get; set; }
}
