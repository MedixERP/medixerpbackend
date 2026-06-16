using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

public class FileService : IFileService
{
    private readonly string _rootPath;

    public FileService()
    {
        _rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    }

    public async Task<string> SaveAvatarAsync(IFormFile file, string? oldUrl = null)
    {
        var folder = Path.Combine(_rootPath, "uploads", "avatars");

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        if (!string.IsNullOrEmpty(oldUrl))
        {
            var oldPath = Path.Combine(_rootPath, oldUrl.TrimStart('/'));

            if (File.Exists(oldPath))
                File.Delete(oldPath);
        }

        var fileName = $"{Guid.NewGuid()}.jpg";
        var fullPath = Path.Combine(folder, fileName);

        using var image = await Image.LoadAsync(file.OpenReadStream());

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(300, 300),
            Mode = ResizeMode.Crop
        }));

        await image.SaveAsJpegAsync(fullPath, new JpegEncoder
        {
            Quality = 90
        });

        return $"/uploads/avatars/{fileName}";
    }
}