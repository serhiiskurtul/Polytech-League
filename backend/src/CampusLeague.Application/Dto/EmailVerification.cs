namespace CampusLeague.Application.Dto;

public class EmailVerificationRequest
{
    public long UserId { get; set; }
    public required string Code { get; set; }

}
