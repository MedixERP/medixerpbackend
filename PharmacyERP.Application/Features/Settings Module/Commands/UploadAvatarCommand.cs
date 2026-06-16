using MediatR;
using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Models;

public class UploadAvatarCommand : IRequest<Result<string>>
{
    public IFormFile File { get; set; }
}