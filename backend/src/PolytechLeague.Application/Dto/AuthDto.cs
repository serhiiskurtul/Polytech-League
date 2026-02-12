namespace PolytechLeague.Application.Dto;

public class AuthRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class AuthResponse
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }

    public void Deconstruct(out string accessToken, out string refreshToken)
    {
        accessToken = AccessToken;
        refreshToken = RefreshToken;
    }

}
