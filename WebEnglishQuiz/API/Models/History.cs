using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class History
    {
        public int Htrno { get; set; }
        public string Username { get; set; } = null!;
        public int Testno { get; set; }
        public int Subjectno { get; set; }
        public int Questionno { get; set; }
        public int Answerno { get; set; }
    }
}
