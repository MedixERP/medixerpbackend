using MediatR;

public class ExportCustomersQuery : IRequest<byte[]>
{
    public string Format { get; set; }
}