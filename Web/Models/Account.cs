using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class Account
{
    [Key]
    public string Username { get; set; }
    public string? Title { get; set; }
    public string AuthToken { get; set; }
    
    public int Id { get; set; }

    public string Uuid { get; set; }
    
    public string Email { get; set; }
}