namespace PolytechLeague.Infrastructure.Email;

public class EmailSettings
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? ApiKey { get; set; }
}
