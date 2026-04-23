using System.Data;
using ExcelDataReader;
using PolytechLeague.Application.Dto;
using PolytechLeague.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace PolytechLeague.Application.Services.Implementations.General;

public class ExcelImportService : IExcelImportService
{
    private readonly ILogger<ExcelImportService> _logger;
    private readonly HttpClient _httpClient;

    public ExcelImportService(ILogger<ExcelImportService> logger)
    {
        _logger = logger;
        _httpClient = new HttpClient();
        // Required for ExcelDataReader on .NET Core
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    }

    public async Task<ScrapedTournamentExcelData> ImportFromChessResultsUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Importing excel from URL: {Url}", url);

        // 1. Convert URL to Excel export URL
        // Example: https://chess-results.com/tnr1259469.aspx?lan=12
        // To: https://chess-results.com/tnr1259469.aspx?lan=12&art=1&prt=4&excel=2010
        
        string excelUrl = url;
        if (!url.Contains("excel=2010"))
        {
            string separator = url.Contains("?") ? "&" : "?";
            excelUrl = $"{url}{separator}art=1&prt=4&excel=2010";
        }

        var response = await _httpClient.GetAsync(excelUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await ImportFromStreamAsync(stream, cancellationToken);
    }

    public async Task<ScrapedTournamentExcelData> ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        using var reader = ExcelReaderFactory.CreateReader(stream);

        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
        {
            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
            {
                UseHeaderRow = false // We'll find headers manually
            }
        });

        var data = new ScrapedTournamentExcelData();
        if (result.Tables.Count > 0)
        {
            var table = result.Tables[0];
            ParseTable(table, data);
        }

        return data;
    }

    private void ParseTable(DataTable table, ScrapedTournamentExcelData data)
    {
        int headerRowIdx = -1;
        int snrIdx = -1;
        int nameIdx = -1;
        int ptsIdx = -1;
        int rkIdx = -1;
        int rtgIdx = -1;
        int fideIdIdx = -1;
        int groupIdx = -1;

        // 1. Find Header Row
        for (int i = 0; i < Math.Min(table.Rows.Count, 15); i++)
        {
            var row = table.Rows[i];
            var rowText = string.Join(" ", row.ItemArray.Select(c => c?.ToString() ?? ""));

            // Look for common headers (English/Ukrainian)
            if (ContainsAny(rowText, new[] { "Name", "Ім'я", "Очки", "Pts.", "Rk.", "Ст.ном", "№" }))
            {
                headerRowIdx = i;
                
                // Identify columns
                for (int j = 0; j < row.ItemArray.Length; j++)
                {
                    string cell = row.ItemArray[j]?.ToString()?.Trim() ?? "";
                    if (string.IsNullOrEmpty(cell)) continue;

                    if (cell.Equals("No.", StringComparison.OrdinalIgnoreCase) || cell.Contains("Ст.ном") || cell.Equals("№"))
                    {
                         // In chess-results art=1, the first column is Rank, second is SNo. Both might be "Ст.ном" in UA.
                         if (rkIdx == -1) rkIdx = j;
                         else if (snrIdx == -1) snrIdx = j;
                    }
                    else if (cell.Equals("Name", StringComparison.OrdinalIgnoreCase) || cell.Contains("Ім'я"))
                    {
                        nameIdx = j;
                    }
                    else if (cell.Contains("Pts.") || cell.Contains("Очки"))
                    {
                        ptsIdx = j;
                    }
                    else if (cell.Contains("Rk.") || cell.Contains("Місце"))
                    {
                        rkIdx = j;
                    }
                    else if (cell.Contains("Rtg") || cell.Contains("Рейт"))
                    {
                        rtgIdx = j;
                    }
                    else if (cell.Contains("FideID") || cell.Contains("Код FIDE"))
                    {
                        fideIdIdx = j;
                    }
                    else if (cell.Contains("Group") || cell.Contains("Група") || cell.Contains("Club") || cell.Contains("Клуб"))
                    {
                        groupIdx = j;
                    }
                }
                break;
            }
        }

        if (headerRowIdx == -1 || nameIdx == -1)
        {
             _logger.LogWarning("Could not find headers in Excel file. RowChecked: {Rows}", table.Rows.Count);
             return;
        }

        // 2. Parse Players
        for (int i = headerRowIdx + 1; i < table.Rows.Count; i++)
        {
            var row = table.Rows[i];
            string name = row.ItemArray[nameIdx]?.ToString()?.Replace(",", " ").Trim() ?? "";
            if (string.IsNullOrEmpty(name)) continue;

            var player = new ExcelPlayer
            {
                FullName = name,
                Rank = rkIdx != -1 ? TryParseInt(row.ItemArray[rkIdx]) : i - headerRowIdx,
                Points = ptsIdx != -1 ? TryParseDouble(row.ItemArray[ptsIdx]) : 0,
                Rating = rtgIdx != -1 ? TryParseInt(row.ItemArray[rtgIdx]) : null,
                FideId = fideIdIdx != -1 ? row.ItemArray[fideIdIdx]?.ToString()?.Trim() : null,
                Group = groupIdx != -1 ? row.ItemArray[groupIdx]?.ToString()?.Trim() : null
            };

            data.Players.Add(player);
        }
    }

    private bool ContainsAny(string text, string[] keywords)
    {
        return keywords.Any(k => text.Contains(k, StringComparison.OrdinalIgnoreCase));
    }

    private int? TryParseInt(object? val)
    {
        if (val == null) return null;
        if (int.TryParse(val.ToString(), out int res)) return res;
        return null;
    }

    private double TryParseDouble(object? val)
    {
        if (val == null) return 0;
        string s = val.ToString()?.Replace(',', '.') ?? "0";
        if (double.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double res)) return res;
        return 0;
    }
}
