using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PolytechLeague.Application.Services.Interfaces;

namespace PolytechLeague.Application.Services.Implementations;

public class FileService(IWebHostEnvironment _environment) : IFileService
{
    public async Task<string> SaveFileAsync(IFormFile file, string folder, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty");

        var contentPath = _environment.WebRootPath;
        var path = Path.Combine(contentPath, "uploads", folder);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var fileNameWithPath = Path.Combine(path, fileName);

        using var stream = new FileStream(fileNameWithPath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);

        return $"/uploads/{folder}/{fileName}";
    }

    public void DeleteFile(string relativePath)
    {
        var contentPath = _environment.WebRootPath;
        var path = Path.Combine(contentPath, relativePath.TrimStart('/'));

        if (File.Exists(path))
            File.Delete(path);
    }
}
