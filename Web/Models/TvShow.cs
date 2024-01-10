using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Web.Models.Interfaces;

namespace Web.Models;

public class TvShow : ISearchable
{
    [Key] 
    public string RatingKey { get; set; }
    public string Key { get; set; }
    public string Guid { get; set; }
    public string Title { get; set; }
    public string LibraryId { get; set; }
    public string ServerId { get; set; }
    
    [InverseProperty("TvShow")]

    public ICollection<Episode> Episodes { get; set; }
}