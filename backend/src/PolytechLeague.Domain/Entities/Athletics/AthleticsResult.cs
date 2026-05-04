namespace PolytechLeague.Domain.Entities.Athletics;

public class AthleticsResult
{
    public long Id { get; set; }
    public long AthleticsEventId { get; set; }
    public AthleticsEvent AthleticsEvent { get; set; } = null!;
    
    public required string DisciplineName { get; set; } // "Підтягування", "Естафета 4х50 метрів"
    
    public long? PlayerId { get; set; }
    public Player? Player { get; set; }
    
    public string? TeamName { get; set; } // For cases like "522 група" in relay
    
    public string? Score { get; set; } // "34 рази", "30.39 сек"
    public int Rank { get; set; } // 1, 2, 3
}
