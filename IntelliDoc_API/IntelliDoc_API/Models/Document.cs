using System;
using System.Collections.Generic;

namespace IntelliDoc_API.Models
{
    public partial class Document
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Version { get; set; }
        public int? CategoryId { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? LatestArchivedDate { get; set; }
        public DateTime? LatestRestoredDate { get; set; }
        public byte[]? Attachment { get; set; }
        public string? Type { get; set; }
        public bool? HaveArchivedDocVersion { get; set; }
        public bool? IsAllVersionsArchived { get; set; }

        public virtual DocumentCategory? Category { get; set; }
    }
}
