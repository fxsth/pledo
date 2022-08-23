using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class ServerConnection
{
    [Key]
    public string Uri { get; set; }
    public string Protocol { get; set; }

    public string Address { get; set; }

    public int Port { get; set; }

    public bool Local { get; set; }

    public bool Relay { get; set; }

    public bool IpV6 { get; set; }
}