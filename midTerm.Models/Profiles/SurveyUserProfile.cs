using AutoMapper;
using midTerm.Data.Entities;
using midTerm.Models.Models.SurveyUser;

namespace midTerm.Models.Profiles
{
    public class SurveyUserProfile : Profile
    {
        public SurveyUserProfile()
        {
            CreateMap<SurveyUser, SurveyUserBaseModel>()
                .ReverseMap();
            CreateMap<SurveyUser, SurveyUserExtended>()
                .ReverseMap();

            CreateMap<SurveyUserCreate, SurveyUser>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.DoB, opt => opt.MapFrom(src => src.DoB))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Answers, opt => opt.Ignore());

            CreateMap<SurveyUserUpdate, SurveyUser>()
                .ForMember(dest=> dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName,opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName,opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.DoB,opt => opt.MapFrom(src => src.DoB))
                .ForMember(dest => dest.Country,opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.Gender,opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.Answers, opt => opt.Ignore());
        }

    }
}