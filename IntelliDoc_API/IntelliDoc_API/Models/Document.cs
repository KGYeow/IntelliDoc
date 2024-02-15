using System;
using System.Collections.Generic;

namespace IntelliDoc_API.Models
{
    public partial class Document
    {
        public Document()
        {
            DocumentVersionHistories = new HashSet<DocumentVersionHistory>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int CategoryId { get; set; }
        public int CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool HaveArchivedDocVersion { get; set; }
        public bool IsAllVersionsArchived { get; set; }

        public virtual DocumentCategory Category { get; set; } = null!;
        public virtual User CreatedBy { get; set; } = null!;
        public virtual ICollection<DocumentVersionHistory> DocumentVersionHistories { get; set; }
    }
}
