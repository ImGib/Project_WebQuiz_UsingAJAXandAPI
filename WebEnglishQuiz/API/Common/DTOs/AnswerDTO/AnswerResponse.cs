namespace API.Common.DTOs.AnswerDTO
{
    public class AnswerResponse
    {
        public int Answerno { get; set; }
        public int Questionno { get; set; }
        public bool Iscorect { get; set; }
        public string? Description { get; set; }
        public bool? Status { get; set; }
    }
}
