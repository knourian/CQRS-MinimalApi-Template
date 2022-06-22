using AutoMapper;

using PEX.Application.Authors.Models;
using PEX.Domain.Model;
namespace PEX.Application.Authors.Profiles;
public class AuthorProfile : Profile
{
    public AuthorProfile()
    {
        CreateMap<AuthorDto, Author>().ReverseMap();
        CreateMap<Author, AuthorGetDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName));
    }
}
