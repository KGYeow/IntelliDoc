using System;
using System.Collections.Generic;

namespace IntelliDoc_API.Models
{
    public partial class DocumentUserAction
    {
        public int UserId { get; set; }
        public int DocumentId { get; set; }
        public bool IsFlagged { get; set; }

        public virtual Document Document { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
