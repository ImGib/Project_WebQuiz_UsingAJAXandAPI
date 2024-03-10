using API.Common.DTOs.SubjectDTO;
using API.Models;
using AutoMapper;

namespace API.Common.Mapper
{
    public class SubjectMapper:Profile
    {
        public SubjectMapper() {
            CreateMap<Subject, SubjectResponse>();
            CreateMap<Subject, SubjectResponseBase>();
            CreateMap<SubjectRequest, Subject>();
            CreateMap<SubjectRequest, SubjectResponse>();
        }
    }
}
