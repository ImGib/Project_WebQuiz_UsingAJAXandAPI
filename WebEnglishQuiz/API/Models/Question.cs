using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class Question
    {
        public Question()
        {
            Answers = new HashSet<Answer>();
        }

        public int Questionno { get; set; }
        public int Subjectno { get; set; }
        public string? Description { get; set; }
        public bool? Status { get; set; }

        public virtual Subject SubjectnoNavigation { get; set; } = null!;
        public virtual ICollection<Answer> Answers { get; set; }
    }
}
