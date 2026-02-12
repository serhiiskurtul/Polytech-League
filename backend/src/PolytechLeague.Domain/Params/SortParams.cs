using System.Text.Json.Serialization;

namespace PolytechLeague.Domain.Params;

public class SortParams
{
    public string? OrderBy { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SortDirection? SortDirection { get; set; }

    public string ToCacheKey()
    {
        return $"orderBy={OrderBy ?? "null"}_" +
               $"dir={SortDirection?.ToString() ?? "null"}";
    }
}

public enum SortDirection
{
    Asc,
    Desc
}
