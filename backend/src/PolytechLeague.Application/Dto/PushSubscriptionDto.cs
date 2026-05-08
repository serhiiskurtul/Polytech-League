namespace PolytechLeague.Application.Dto;

public class PushSubscriptionDto
{
    public string Endpoint { get; set; }
    public KeysDto Keys { get; set; }
}

public class KeysDto
{
    public string P256dh { get; set; }
    public string Auth { get; set; }
}
