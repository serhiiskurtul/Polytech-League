using System.Text.Json.Serialization;

namespace PolytechLeague.Domain.Params;

public class PageResult<T>
{
    public List<T> Data { get; }
    public int TotalCount { get; }

    [JsonConstructor]
    public PageResult(List<T> data, int totalCount)
    {
        Data = data;
        TotalCount = totalCount;
    }
}
