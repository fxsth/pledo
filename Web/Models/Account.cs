using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class Account
{
    [Key]
    public string Username { get; set; }
    public string Title { get; set; }
    public string Password { get; set; }
    public string UserToken { get; set; }
}