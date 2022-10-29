using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models;

public class KeyValueSetting
{
    [Key]
    public string Key { get; set; }
    public string Value { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? LastModified { get; set; }
}