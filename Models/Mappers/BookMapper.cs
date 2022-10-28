using AutoMapper;
using WebLibrary.Models.Dtos;

namespace WebLibrary.Models.Mappers
{
    public class BookMapper : Profile
    {
        public BookMapper()
        {
            CreateMap<Book,BookDto>().ReverseMap();
        }
    }
}
