using Microsoft.AspNetCore.Http;

namespace PolytechLeague.Application.Services.Interfaces;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile file, string folder, CancellationToken cancellationToken = default);
    void DeleteFile(string relativePath);
}
