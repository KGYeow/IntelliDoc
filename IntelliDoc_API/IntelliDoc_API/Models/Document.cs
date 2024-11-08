﻿using System;
using System.Collections.Generic;

namespace IntelliDoc_API.Models
{
    public partial class Document
    {
        public Document()
        {
            DocumentRelationshipDocumentMains = new HashSet<DocumentRelationship>();
            DocumentRelationshipDocumentRelateds = new HashSet<DocumentRelationship>();
            DocumentUserActions = new HashSet<DocumentUserAction>();
            DocumentVersionHistories = new HashSet<DocumentVersionHistory>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Category { get; set; } = null!;
        public int CurrentVersion { get; set; }
        public int? CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedById { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Type { get; set; } = null!;
        public bool HaveArchivedDocVersion { get; set; }
        public bool IsAllVersionsArchived { get; set; }
        public bool IsRelatedDoc { get; set; }

        public virtual User? CreatedBy { get; set; }
        public virtual User? ModifiedBy { get; set; }
        public virtual ICollection<DocumentRelationship> DocumentRelationshipDocumentMains { get; set; }
        public virtual ICollection<DocumentRelationship> DocumentRelationshipDocumentRelateds { get; set; }
        public virtual ICollection<DocumentUserAction> DocumentUserActions { get; set; }
        public virtual ICollection<DocumentVersionHistory> DocumentVersionHistories { get; set; }
    }
}
