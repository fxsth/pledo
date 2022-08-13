using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class Server
{
    [Key]
    public string Id { get; set; }
    public string Name { get; set; }
    public ICollection<ServerConnection> Connections { get; set; }
    public Uri LastKnown { get; set; }
}