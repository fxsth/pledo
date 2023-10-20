using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models;

public class Server
{
    [Key]
    public string Id { get; set; }
    public string Name { get; set; }
    public string? SourceTitle { get; set; }
    public int? OwnerId { get; set; }
    
    [InverseProperty("Server")]
    public ICollection<ServerConnection> Connections { get; set; }
    public string? LastKnownUri { get; set; }
    
    public bool IsOnline { get; set; }
    
    public string? AccessToken { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset? LastModified { get; set; }
}