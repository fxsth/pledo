using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models;

public class Library
{
    [Key]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Key { get; set; }
    
    [ForeignKey("Server")]
    public string ServerId { get; set; }
    public Server Server { get; set; }
}