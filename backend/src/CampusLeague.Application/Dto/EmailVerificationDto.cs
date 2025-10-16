namespace CampusLeague.Application.Dto;

public class EmailVerificationRequest
{
    public required string Email { get; set; }
    public required string Code { get; set; }

}
