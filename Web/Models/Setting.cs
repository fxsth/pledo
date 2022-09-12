using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class Setting
{
    [Key]
    public string Key { get; set; }
    public string Value { get; set; }
    public DateTime LastModified { get; set; }
}