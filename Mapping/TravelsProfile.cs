using AutoMapper;
using travels_server_side.Entities;
using travels_server_side.Models;

namespace travels_server_side.Mapping
{
    public class TravelsProfile : Profile
    {
        public TravelsProfile()
        {
            CreateMap<SitesEO, SitesDTO>().
                //ForMember(dest => dest.siteAverageRating, opt => opt.MapFrom(src => src.averageRating)).
                ForMember(dest => dest.managerEmail, opt => opt.MapFrom(src => src.managerEmailFK)).
                ReverseMap();

            CreateMap<VisitsEO, VisitsDTO>().
                ForMember(dest => dest.siteId, opt => opt.MapFrom(src => src.siteIdFK)).
                //ForMember(dest => dest.userEmail, opt => opt.MapFrom(src => src.userEmailFK)).
                ForMember(dest => dest.travelId, opt => opt.MapFrom(src => src.travelIdFK)).
                ReverseMap();//דוגמא עם שמות שדות שונים

            //ForMember(dest => dest.travelId, opt => opt.MapFrom(src => src.travelId));//דוגמא עם מיפוי מפורש

            CreateMap<CategoriesEO, SiteCategoriesDTO>().ReverseMap();
            CreateMap<AdminEO, AdminDTO>().ReverseMap();
            CreateMap<ManagersEO, ManagersDTO>().ReverseMap();
            //CreateMap<SitesEO, SitesDTO>().ReverseMap();
            CreateMap<UserPreferencesEO, UserPreferencesDTO>().ReverseMap();
            CreateMap<SiteRatingsEO, SiteRatingsDTO>().ReverseMap();
        }
    }
}
