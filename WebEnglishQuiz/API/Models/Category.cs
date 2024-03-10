using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class Category
    {
        public Category()
        {
            Subjects = new HashSet<Subject>();
        }

        public int Categoryno { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<Subject> Subjects { get; set; }
    }
}
