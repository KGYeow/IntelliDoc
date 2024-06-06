using System;
using System.Collections.Generic;

namespace IntelliDoc_API.Models
{
    public partial class UserManualDocument
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public byte[] Attachment { get; set; } = null!;
    }
}
