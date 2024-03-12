namespace API.Common.DTOs.SubjectDTO
{
    public class SubjectResponseBase
    {
        public int Subjectno { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int Categoryno { get; set; }
        public bool? Status { get; set; }
        public string? Image { get; set; }
    }
}
