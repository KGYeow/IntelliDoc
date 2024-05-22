using System;
using System.Collections.Generic;

namespace IntelliDoc_API.Models
{
    public partial class DocumentRelationship
    {
        public int Id { get; set; }
        public int DocumentMainId { get; set; }
        public int DocumentRelatedId { get; set; }

        public virtual Document DocumentMain { get; set; } = null!;
        public virtual Document DocumentRelated { get; set; } = null!;
    }
}
