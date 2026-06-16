using Microsoft.AspNetCore.Http;

public interface IFileService
{
    Task<string> SaveAvatarAsync(IFormFile file, string? oldUrl = null);
}