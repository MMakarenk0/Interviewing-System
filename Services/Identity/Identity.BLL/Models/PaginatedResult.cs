namespace Identity.BLL.Models;

public class PaginatedResult<T>
{
    public IList<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
}

