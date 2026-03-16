namespace PolytechLeague.Domain.Entities.Checkers;

public class CheckersEventParticipant
{
    public long Id { get; set; }
    public long CheckersEventId { get; set; }
    public CheckersEvent Event { get; set; } = null!;
    public long UserId { get; set; }
    public User User { get; set; } = null!; // передбачається, що базовий клас User існує

    // Поточний рейтинг гравця у шашках на момент початку турніру
    public int StartingRating { get; set; }
    
    // Статистика гравця у цьому конкретному турнірі
    public double PointsEarned { get; set; } = 0;
    //public double BuchholzTieBreak { get; set; } = 0; // Для швейцарської системи
}
