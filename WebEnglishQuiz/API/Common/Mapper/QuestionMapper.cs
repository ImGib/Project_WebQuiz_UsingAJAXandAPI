using API.Common.DTOs.QuestionDTO;
using API.Models;
using AutoMapper;

namespace API.Common.Mapper
{
    public class QuestionMapper:Profile
    {
        public QuestionMapper()
        {
            CreateMap<Question, QuestionResponse>();
            CreateMap<QuestionRequestBase, Question>();
        }
    }
}
