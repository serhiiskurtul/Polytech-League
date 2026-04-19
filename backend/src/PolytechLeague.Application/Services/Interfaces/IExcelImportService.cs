using PolytechLeague.Application.Dto;

namespace PolytechLeague.Application.Services.Interfaces;

public interface IExcelImportService
{
    Task<ScrapedTournamentExcelData> ImportFromChessResultsUrlAsync(string url, CancellationToken cancellationToken = default);
    Task<ScrapedTournamentExcelData> ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken = default);
}
