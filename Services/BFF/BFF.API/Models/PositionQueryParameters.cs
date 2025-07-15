namespace BFF.API.Models;

public class PositionQueryParameters
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? TitleFilter { get; set; }
    public bool? IsActive { get; set; }
}