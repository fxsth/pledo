using Web.Models.Interfaces;

namespace Web.Models.DTO;

public class ResultResource<T>
{
    public int TotalItems { get; set; }
    public IEnumerable<T> Items { get; set; }
}