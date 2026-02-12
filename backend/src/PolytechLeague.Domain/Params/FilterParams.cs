namespace PolytechLeague.Domain.Params;

public class FilterParams
{
    public string? Name { get; set; }

    public string ToCacheKey()
    {
        return $"name={Name ?? "null"}_";
    }
}
