using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Configuration;
using PolytechLeague.Application.Services.Interfaces;

namespace PolytechLeague.Application.Services.Implementations;

public class CloudflareR2FileService : IFileService
{
    private readonly IAmazonS3 _s3Client;
    private readonly CloudflareR2Settings _settings;
    private readonly ILogger<CloudflareR2FileService> _logger;

    public CloudflareR2FileService(IOptions<CloudflareR2Settings> settings, ILogger<CloudflareR2FileService> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        if (string.IsNullOrEmpty(_settings.AccessKey) || string.IsNullOrEmpty(_settings.SecretKey))
        {
            throw new ArgumentException("Cloudflare R2 AccessKey or SecretKey is missing. Check environment variables.");
        }

        if (string.IsNullOrEmpty(_settings.ServiceUrl))
        {
            throw new ArgumentException("Cloudflare R2 ServiceUrl is missing. Check environment variables.");
        }

        if (string.IsNullOrEmpty(_settings.BucketName))
        {
            throw new ArgumentException("Cloudflare R2 BucketName is missing. Check environment variables.");
        }
        
        var config = new AmazonS3Config
        {
            ServiceURL = _settings.ServiceUrl,
            AuthenticationRegion = "us-east-1", // More reliable for R2 than "auto"
            ForcePathStyle = true,
            Timeout = TimeSpan.FromMinutes(5) // Increase timeout for large videos
        };

        _s3Client = new AmazonS3Client(_settings.AccessKey, _settings.SecretKey, config);
    }

    public async Task<string> SaveFileAsync(IFormFile file, string folder, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty");

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var key = string.IsNullOrEmpty(folder) ? fileName : $"{folder}/{fileName}";

        _logger.LogInformation("[R2] Starting upload for key: {Key}. Size: {Size} bytes", key, file.Length);
        
        try 
        {
            using var stream = file.OpenReadStream();
            
            var request = new PutObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = key,
                InputStream = stream,
                ContentType = file.ContentType,
                DisablePayloadSigning = true,
                AutoCloseStream = true
            };

            _logger.LogInformation("[R2] Sending PutObjectRequest to S3 endpoint...");
            var response = await _s3Client.PutObjectAsync(request, cancellationToken);
            
            _logger.LogInformation("[R2] Upload successful. RequestId: {RequestId}, Status: {Status}", 
                response.ResponseMetadata.RequestId, response.HttpStatusCode);

            var finalUrl = $"{_settings.PublicUrl}/{key}";
            _logger.LogInformation("[R2] Generated public URL: {Url}", finalUrl);
            
            return finalUrl;
        }
        catch (AmazonS3Exception e)
        {
            _logger.LogError(e, "[R2] AmazonS3Exception during upload for key {Key}: {Message}", key, e.Message);
            throw new Exception($"S3 Error: {e.Message}", e);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[R2] Unexpected error during upload for key {Key}: {Message}", key, e.Message);
            throw;
        }
    }

    public void DeleteFile(string relativePath)
    {
        // For R2, we extract the key from the full URL or relative path
        // Since SaveFileAsync now returns a full URL, we need to handle that.
        
        string key;
        if (Uri.TryCreate(relativePath, UriKind.Absolute, out var uri))
        {
            key = uri.AbsolutePath.TrimStart('/');
        }
        else
        {
            key = relativePath.TrimStart('/');
        }

        // AWS SDK DeleteObject is async, but IFileService defines it as sync.
        // We'll use Task.Run or change the interface if possible.
        // Given I want to minimize breaking changes, I'll use a Fire-and-Forget or Wait.
        // Better yet: Let's check if we can make IFileService async for deletion too.
        
        _s3Client.DeleteObjectAsync(_settings.BucketName, key).GetAwaiter().GetResult();
    }
}
