namespace API.Common.DTOs.HistoryDTO
{
    public class HistoryResponse
    {
        public int Htrno { get; set; }
        public string Username { get; set; } = null!;
        public int Testno { get; set; }
        public int Subjectno { get; set; }
        public int Questionno { get; set; }
        public int Answerno { get; set; }
    }
}
