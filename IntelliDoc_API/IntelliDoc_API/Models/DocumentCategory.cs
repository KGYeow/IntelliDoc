﻿using System;
using System.Collections.Generic;

namespace IntelliDoc_API.Models
{
    public partial class DocumentCategory
    {
        public DocumentCategory()
        {
            DocumentVersionHistories = new HashSet<DocumentVersionHistory>();
            Documents = new HashSet<Document>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<DocumentVersionHistory> DocumentVersionHistories { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
    }
}