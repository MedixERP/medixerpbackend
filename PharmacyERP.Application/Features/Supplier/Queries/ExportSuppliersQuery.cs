using MediatR;

public class ExportSuppliersQuery : IRequest<byte[]>
{
    public string Format { get; set; }
}