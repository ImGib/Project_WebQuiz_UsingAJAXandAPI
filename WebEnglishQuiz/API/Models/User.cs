﻿using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class User
    {
        public User()
        {
            Subjectnos = new HashSet<Subject>();
        }

        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phonenumber { get; set; }
        public bool? Role { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<Subject> Subjectnos { get; set; }
    }
}
