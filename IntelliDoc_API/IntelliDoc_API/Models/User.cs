using System;
using System.Collections.Generic;

namespace IntelliDoc_API.Models
{
    public partial class User
    {
        public User()
        {
            DocumentVersionHistories = new HashSet<DocumentVersionHistory>();
            Documents = new HashSet<Document>();
            Notifications = new HashSet<Notification>();
        }

        public int Id { get; set; }
        public int UserRoleId { get; set; }
        public string FullName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public byte[]? ProfilePhoto { get; set; }
        public string Password { get; set; } = null!;

        public virtual UserRole UserRole { get; set; } = null!;
        public virtual ICollection<DocumentVersionHistory> DocumentVersionHistories { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
