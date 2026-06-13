using MediatR;

public class ExportProductsQuery : IRequest<byte[]>
{
    public string Format { get; set; }
}