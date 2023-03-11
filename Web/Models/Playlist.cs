using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models;

public class Playlist
{
    [Key]
    public string Id { get; set; }
    public string Name { get; set; }
    public List<string> Items { get; set; }
    [ForeignKey("Server")]
    public string ServerId { get; set; }
    public Server Server { get; set; }
}