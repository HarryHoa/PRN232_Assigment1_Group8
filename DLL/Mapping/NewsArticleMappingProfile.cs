using AutoMapper;
using Common.Dto;
using DAL.Models;

namespace DLL.Mapping
{
    public class NewsArticleMappingProfile : Profile
    {
        public NewsArticleMappingProfile()
        {
            // NewsArticle to NewsArticleDto
            CreateMap<NewsArticle, NewsArticleDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.NewsTitle))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.NewsContent))
                .ForMember(dest => dest.Source, opt => opt.MapFrom(src => src.NewsSource))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId ?? 0))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryDesciption))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.NewsStatus ?? true))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy.AccountName))
                .ForMember(dest => dest.TagIds, opt => opt.MapFrom(src => src.Tags.Select(t => t.TagId).ToList()))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));

            // NewsArticleCreateDto to NewsArticle
            CreateMap<NewsArticleCreateDto, NewsArticle>()
                .ForMember(dest => dest.NewsTitle, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.NewsContent, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.NewsSource, opt => opt.MapFrom(src => src.Source))
                .ForMember(dest => dest.NewsStatus, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Tags, opt => opt.Ignore()) // Handle tags separately
                .ForMember(dest => dest.NewsArticleId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

            // NewsArticleUpdateDto to NewsArticle
            CreateMap<NewsArticleUpdateDto, NewsArticle>()
                .ForMember(dest => dest.NewsTitle, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.NewsContent, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.NewsSource, opt => opt.MapFrom(src => src.Source))
                .ForMember(dest => dest.NewsStatus, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Tags, opt => opt.Ignore()) // Handle tags separately
                .ForMember(dest => dest.NewsArticleId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

            // Tag to TagDto
            CreateMap<Tag, TagDto>();

            // Category to CategoryDto
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryDesciption));
        }
    }
}