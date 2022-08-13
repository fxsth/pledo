using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class Library
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Type { get; set; }
}