using MediatR;
using PharmacyERP.Application.Common.Models;

public class PrintBarcodeLabelQuery : IRequest<Result<byte[]>>
{
    public int ProductId { get; set; }
}