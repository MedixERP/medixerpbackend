using MediatR;

public class ExportExpiredBatchesQuery : IRequest<byte[]>
{
    public string Format { get; set; }
}