using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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