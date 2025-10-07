namespace CampusLeague.Domain.Params;

public class FilterParams
{
    public string? Name { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    public string ToCacheKey()
    {
        return $"name={Name ?? "null"}_" +
               $"min={MinPrice?.ToString("F2") ?? "null"}_" + //F2 - Форматування числа з фіксованими 2 знаками після коми (Fixed-point, 2 decimals).
               $"max={MaxPrice?.ToString("F2") ?? "null"}";
    }
}
