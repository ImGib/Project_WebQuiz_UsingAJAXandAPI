using API.Common.DTOs.AnswerDTO;
using API.Models;

namespace API.Common.DTOs.QuestionDTO
{
    public class QuestionResponse
    {
        public int Questionno { get; set; }
        public int Subjectno { get; set; }
        public string? Description { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<AnswerResponse> Answers { get; set; }
    }
}
