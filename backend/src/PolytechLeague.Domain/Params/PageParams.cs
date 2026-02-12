namespace PolytechLeague.Domain.Params;

public class PageParams
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }

    public string ToCacheKey()
    {
        return $"page={Page ?? 1}_size={PageSize ?? 10}";
    }
}
