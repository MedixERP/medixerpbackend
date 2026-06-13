using MediatR;

public class ExportInvoicesQuery : IRequest<byte[]>
{
    public string Format { get; set; } 
}