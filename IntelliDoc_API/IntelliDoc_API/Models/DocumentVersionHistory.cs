using System;
using System.Collections.Generic;

namespace IntelliDoc_API.Models
{
    public partial class DocumentVersionHistory
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string Version { get; set; } = null!;
        public int UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? ArchivedDate { get; set; }
        public byte[] Attachment { get; set; } = null!;
        public string Type { get; set; } = null!;
        public bool IsArchived { get; set; }

        public virtual User UpdatedBy { get; set; } = null!;
    }
}
