using System;
using System.Collections.Generic;

namespace IntelliDoc_API.Models
{
    public partial class UserMongo
    {
        public UserMongo()
        {
            DocumentCreatedBies = new HashSet<Document>();
            DocumentModifiedBies = new HashSet<Document>();
            DocumentUserActions = new HashSet<DocumentUserAction>();
            DocumentVersionHistories = new HashSet<DocumentVersionHistory>();
            Notifications = new HashSet<Notification>();
        }

        public int Id { get; set; }
        public int UserRoleId { get; set; }
        public string FullName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public byte[]? ProfilePhoto { get; set; }
        public string Password { get; set; } = null!;
        public bool IsActive { get; set; }

        public virtual UserRole UserRole { get; set; } = null!;
        public virtual ICollection<Document> DocumentCreatedBies { get; set; }
        public virtual ICollection<Document> DocumentModifiedBies { get; set; }
        public virtual ICollection<DocumentUserAction> DocumentUserActions { get; set; }
        public virtual ICollection<DocumentVersionHistory> DocumentVersionHistories { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
