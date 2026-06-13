using MediatR;

public class ExportExpiredProductsQuery : IRequest<byte[]>
{
    public string Format { get; set; }
}