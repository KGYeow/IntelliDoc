using System;
using System.Collections.Generic;

namespace IntelliDoc_API.Models
{
    public partial class DocumentVersionHistory
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int Version { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? ArchivedDate { get; set; }
        public byte[] Attachment { get; set; } = null!;
        public string Type { get; set; } = null!;
        public bool IsArchived { get; set; }

        public virtual Document Document { get; set; } = null!;
        public virtual User? UpdatedBy { get; set; }
    }
}
