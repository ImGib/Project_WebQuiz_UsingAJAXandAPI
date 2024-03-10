using API.Common.DTOs.UserDTO;
using API.Models;
using AutoMapper;

namespace API.Common.Mapper
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<UserRegister, User>();
            CreateMap<User, UserDisplay>();
        }
    }
}
