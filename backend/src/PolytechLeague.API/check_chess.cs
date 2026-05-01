using Microsoft.EntityFrameworkCore;
using PolytechLeague.Persistence.Database;
using PolytechLeague.Domain.Entities.Chess;

// This is just to check if games exist
public class Checker {
    public static async Task Check(ApplicationDbContext db) {
        var gamesCount = await db.ChessGames.CountAsync();
        var participantsCount = await db.ChessEventParticipants.CountAsync();
        Console.WriteLine($"GAMES: {gamesCount}, PARTICIPANTS: {participantsCount}");
    }
}
