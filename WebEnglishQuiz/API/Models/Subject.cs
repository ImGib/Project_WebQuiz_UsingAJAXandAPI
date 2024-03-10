using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class Subject
    {
        public Subject()
        {
            Questions = new HashSet<Question>();
            Usernames = new HashSet<User>();
        }

        public int Subjectno { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int Categoryno { get; set; }
        public bool? Status { get; set; }

        public virtual Category CategorynoNavigation { get; set; } = null!;
        public virtual ICollection<Question> Questions { get; set; }

        public virtual ICollection<User> Usernames { get; set; }
    }
}
