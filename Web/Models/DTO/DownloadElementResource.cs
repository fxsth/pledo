namespace Web.Models.DTO
{
    public class DownloadElementResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public bool FinishedSuccessfully { get; set; }
        public ElementType ElementType { get; set; }
        public double Progress { get; set; }
        public long TotalBytes { get; set; }
        public long DownloadedBytes { get; set; }
        public DateTimeOffset Started { get; set; }
        public DateTimeOffset Finished { get; set; }
    }
}
