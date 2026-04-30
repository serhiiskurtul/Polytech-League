namespace PolytechLeague.Application.Configuration;

public class CloudflareR2Settings
{
    public string BucketName { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string ServiceUrl { get; set; } = string.Empty;
    public string PublicUrl { get; set; } = string.Empty;
}
