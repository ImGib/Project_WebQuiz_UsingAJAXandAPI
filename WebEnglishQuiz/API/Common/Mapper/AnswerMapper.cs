using API.Common.DTOs.AnswerDTO;
using API.Models;
using AutoMapper;

namespace API.Common.Mapper
{
    public class AnswerMapper:Profile
    {
        public AnswerMapper() {
            CreateMap<Answer, AnswerResponse>();
            CreateMap<AnswerRequestBase, Answer>();
            CreateMap<AnswerRequestPut, Answer>();
            CreateMap<AnswerRequestPut, AnswerResponse>();
        }
    }
}
