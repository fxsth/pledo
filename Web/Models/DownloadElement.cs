using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Web.Models
{
    public class DownloadElement
    {
        public DownloadElement()
        {
            CancellationTokenSource = new CancellationTokenSource();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public bool FinishedSuccessfully { get; set; }
        public ElementType ElementType { get; set; }
        public double Progress { get; set; } = 0;
        public long TotalBytes { get; set; }
        public long DownloadedBytes { get; set; }
        public DateTimeOffset? Started { get; set; }
        public DateTimeOffset? Finished { get; set; }
        public string MediaKey { get; set; }
        
        [JsonIgnore]
        [NotMapped]
        public HttpRequestMessage RequestMessage { get; set; }

        [NotMapped]
        public CancellationTokenSource CancellationTokenSource { get; set; }
    }

    public enum ElementType
    {
        Movie,
        TvShow
    }
}
