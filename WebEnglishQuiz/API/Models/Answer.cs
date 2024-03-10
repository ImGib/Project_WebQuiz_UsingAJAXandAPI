using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class Answer
    {
        public int Answerno { get; set; }
        public int Questionno { get; set; }
        public bool Iscorect { get; set; }
        public string? Description { get; set; }
        public bool? Status { get; set; }

        public virtual Question QuestionnoNavigation { get; set; } = null!;
    }
}
