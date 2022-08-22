﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public bool FinishedSuccessfully { get; set; }
        public ElementType ElementType { get; set; }
        public IProgress<double> Progress { get; set; }
        public long TotalBytes { get; set; }
        
        [NotMapped]
        public CancellationTokenSource CancellationTokenSource { get; set; }
    }

    public enum ElementType
    {
        Movie,
        TvShow
    }
}