using AutoMapper;
using Common.Dto;
using DAL.Models;

namespace DLL;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<NewsArticle, NewArticleRes>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName));

        CreateMap<SystemAccount, AdminCRUDdto>().ReverseMap();


    }
}