﻿using System;
using System.Collections.Generic;

namespace IntelliDoc_API.Models
{
    public partial class RoleAccessPage
    {
        public int Id { get; set; }
        public int UserRoleId { get; set; }
        public int PageId { get; set; }

        public virtual Page Page { get; set; } = null!;
        public virtual UserRole UserRole { get; set; } = null!;
    }
}
