using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class BusyTask
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public TaskType Type { get; set; }
    public double Progress { get; set; }
    public bool Completed { get; set; }
}