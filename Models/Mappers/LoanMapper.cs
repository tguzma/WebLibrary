using AutoMapper;
using WebLibrary.Models.Dtos;

namespace WebLibrary.Models.Mappers
{
    public class LoanMapper : Profile
    {
        public LoanMapper() { 
        
            CreateMap<Loan,LoanDto>().ReverseMap();
        }
    }
}
