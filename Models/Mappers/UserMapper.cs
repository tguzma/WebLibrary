using AutoMapper;
using WebLibrary.Models.Dtos;

namespace WebLibrary.Models.Mappers
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
