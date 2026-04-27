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
        _logger.LogInformation("Excel file opened. Found {TableCount} sheets.", result.Tables.Count);

        foreach (DataTable table in result.Tables)
        {
            _logger.LogInformation("Checking sheet: {TableName} with {RowCount} rows.", table.TableName, table.Rows.Count);
            ParseTable(table, data);
            if (data.Players.Count > 0)
            {
                _logger.LogInformation("Found {PlayerCount} players in sheet {TableName}.", data.Players.Count, table.TableName);
                break;
            }
        }

        return data;
    }

    private void ParseTable(DataTable table, ScrapedTournamentExcelData data)
    {
        int headerRowIdx = -1;
        int snrIdx = -1;
        int nameIdx = -1;
        int firstNameIdx = -1;
        int lastNameIdx = -1;
        int ptsIdx = -1;
        int rkIdx = -1;
        int rtgIdx = -1;
        int fideIdIdx = -1;
        int groupIdx = -1;
        int titleIdx = -1;

        var headerKeywords = new[] { 
            "Name", "Ім'я", "Прізвище", "ПІБ", "Очки", "Pts.", "Rk.", "Ст.ном", "№", 
            "Result", "Результат", "Рейтинг", "Rating", "Rank", "Points", "Score", 
            "Бали", "FIDE", "ID", "Учасник", "Гравець", "Participant", "Player",
            "Surname", "Last Name", "First Name", "Group", "Група", "Клас", "Клуб", "Club", "Учасники",
            "F.I.O.", "Ф.И.О.", "П.І.Б.", "FIO", "ФИО", "Имя"
        };

        // 1. Find Header Row
        // Increased to 100 rows to handle files with lots of metadata at the top
        for (int i = 0; i < Math.Min(table.Rows.Count, 100); i++)
        {
            var row = table.Rows[i];
            var rowText = string.Join(" ", row.ItemArray.Select(c => c?.ToString() ?? ""));

            // Look for common headers (English/Ukrainian)
            if (ContainsAny(rowText, headerKeywords))
            {
                _logger.LogInformation("Potential header found at row {RowIdx}: {RowText}", i, rowText);
                int currentNameIdx = -1;
                int currentFirstNameIdx = -1;
                int currentLastNameIdx = -1;
                int currentPtsIdx = -1;
                int currentRkIdx = -1;
                int currentRtgIdx = -1;
                int currentFideIdIdx = -1;
                int currentGroupIdx = -1;
                int currentSnrIdx = -1;
                int currentTitleIdx = -1;

                // Identify columns
                for (int j = 0; j < row.ItemArray.Length; j++)
                {
                    string cell = row.ItemArray[j]?.ToString()?.Trim() ?? "";
                    if (string.IsNullOrEmpty(cell)) continue;

                    string cellLower = cell.ToLower();

                    if (cellLower.Equals("no.") || cellLower.Contains("ст.ном") || cellLower.Equals("№") || cellLower.Equals("n") || cellLower.Equals("sno"))
                    {
                         if (currentRkIdx == -1) currentRkIdx = j;
                         else if (currentSnrIdx == -1) currentSnrIdx = j;
                    }
                    else if (cellLower.Equals("name") || cellLower.Contains("піб") || cellLower.Contains("учасник") || cellLower.Contains("гравець") || cellLower.Contains("player") || cellLower.Contains("participant") || cellLower.Equals("имя") || cellLower.Equals("імя") || cellLower.Equals("ім'я"))
                    {
                        currentNameIdx = j;
                    }
                    else if (cellLower.Contains("ім'я") || cellLower.Contains("ім’я") || cellLower.Contains("імя") || cellLower.Contains("имя") || cellLower.Contains("first name"))
                    {
                        currentFirstNameIdx = j;
                    }
                    else if (cellLower.Contains("прізвище") || cellLower.Contains("last name") || cellLower.Contains("surname"))
                    {
                        currentLastNameIdx = j;
                    }
                    else if (cellLower.Contains("pts.") || cellLower.Contains("очки") || cellLower.Contains("боли") || cellLower.Contains("бали") || cellLower.Contains("points") || cellLower.Contains("score") || cellLower.Equals("о") || cellLower.Equals("б") || cellLower.Contains("result"))
                    {
                        currentPtsIdx = j;
                    }
                    else if (cellLower.Contains("rk.") || cellLower.Contains("місце") || cellLower.Contains("поз") || cellLower.Contains("rank"))
                    {
                        currentRkIdx = j;
                    }
                    else if (cellLower.Contains("rtg") || cellLower.Contains("рейт") || cellLower.Contains("elo") || cellLower.Contains("рейтинг"))
                    {
                        currentRtgIdx = j;
                    }
                    else if (cellLower.Contains("fideid") || cellLower.Contains("код fide") || cellLower.Contains("fide"))
                    {
                        currentFideIdIdx = j;
                    }
                    else if (cellLower.Contains("group") || cellLower.Contains("група") || cellLower.Contains("club") || cellLower.Contains("клуб") || cellLower.Contains("клас") || cellLower.Contains("спец"))
                    {
                        currentGroupIdx = j;
                    }
                    else if (cellLower.Contains("title") || cellLower.Contains("зван") || cellLower.Contains("титул"))
                    {
                        currentTitleIdx = j;
                    }
                }

                // A valid header row MUST have at least a Name column or both First/Last name columns
                if (currentNameIdx != -1 || (currentFirstNameIdx != -1 && currentLastNameIdx != -1))
                {
                    headerRowIdx = i;
                    nameIdx = currentNameIdx;
                    firstNameIdx = currentFirstNameIdx;
                    lastNameIdx = currentLastNameIdx;
                    ptsIdx = currentPtsIdx;
                    rkIdx = currentRkIdx;
                    rtgIdx = currentRtgIdx;
                    fideIdIdx = currentFideIdIdx;
                    groupIdx = currentGroupIdx;
                    snrIdx = currentSnrIdx;
                    titleIdx = currentTitleIdx;
                    _logger.LogInformation("Header row found at index {Index} in sheet {Sheet}. NameIdx: {NameIdx}, First: {F}, Last: {L}", i, table.TableName, currentNameIdx, currentFirstNameIdx, currentLastNameIdx);
                    break;
                }
            }
        }

        if (headerRowIdx == -1 || (nameIdx == -1 && (firstNameIdx == -1 || lastNameIdx == -1)))
        {
             _logger.LogWarning("Could not find headers in sheet {Sheet}. RowChecked: {Rows}", table.TableName, table.Rows.Count);
             // DEBUG: Log the first 5 rows to see what's going on
             for (int i = 0; i < Math.Min(table.Rows.Count, 5); i++)
             {
                 var row = table.Rows[i];
                 var rowText = string.Join(" | ", row.ItemArray.Select(c => c?.ToString() ?? ""));
                 _logger.LogInformation("Row {RowIdx}: {RowText}", i, rowText);
             }
             return;
        }

        // 2. Parse Players
        for (int i = headerRowIdx + 1; i < table.Rows.Count; i++)
        {
            var row = table.Rows[i];
            
            string name = "";
            if (nameIdx != -1)
            {
                name = row.ItemArray[nameIdx]?.ToString()?.Replace(",", " ").Trim() ?? "";
            }

            if (firstNameIdx != -1 && lastNameIdx != -1)
            {
                string fn = row.ItemArray[firstNameIdx]?.ToString()?.Trim() ?? "";
                string ln = row.ItemArray[lastNameIdx]?.ToString()?.Trim() ?? "";
                
                if (string.IsNullOrEmpty(name))
                {
                    name = $"{ln} {fn}".Trim();
                }
                else if (!name.Contains(fn) || !name.Contains(ln))
                {
                    // Prefer combined if we have both and nameIdx was just one of them
                    name = $"{ln} {fn}".Trim();
                }
            }

            if (string.IsNullOrEmpty(name)) continue;

            var player = new ExcelPlayer
            {
                FullName = name,
                Rank = rkIdx != -1 ? TryParseInt(row.ItemArray[rkIdx]) : i - headerRowIdx,
                Points = ptsIdx != -1 ? TryParseDouble(row.ItemArray[ptsIdx]) : 0,
                Rating = rtgIdx != -1 ? TryParseInt(row.ItemArray[rtgIdx]) : null,
                FideId = fideIdIdx != -1 ? row.ItemArray[fideIdIdx]?.ToString()?.Trim() : null,
                Group = groupIdx != -1 ? row.ItemArray[groupIdx]?.ToString()?.Trim() : null,
                Title = titleIdx != -1 ? row.ItemArray[titleIdx]?.ToString()?.Trim() : null
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
