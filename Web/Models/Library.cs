using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class Library
{
    [Key]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Key { get; set; }
}