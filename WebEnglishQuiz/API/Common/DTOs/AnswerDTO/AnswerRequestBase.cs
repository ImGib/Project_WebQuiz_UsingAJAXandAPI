namespace API.Common.DTOs.AnswerDTO
{
    public class AnswerRequestBase
    {
        public int Questionno { get; set; }
        public bool Iscorect { get; set; }
        public string? Description { get; set; }
    }
}
