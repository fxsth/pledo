using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class PlexAccount
{
    [Key]
    public string Username { get; set; }
    public string Password { get; set; }
    public string AuthKey { get; set; }
}