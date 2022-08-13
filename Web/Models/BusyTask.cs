using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class BusyTask
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public TaskType Type { get; set; }
}