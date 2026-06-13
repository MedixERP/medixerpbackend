using MediatR;

public class ExportMonthlySalesQuery : IRequest<byte[]>
{
    public string Format { get; set; }
}