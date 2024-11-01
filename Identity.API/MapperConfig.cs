using AutoMapper;
using Entity;
using Identity.API.BusinessObjects.CourtOwnerModel;
using Identity.API.BusinessObjects.UserViewModel;


namespace Identity.API;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<User, UserViewDto>().ReverseMap();
         
        CreateMap<UserCreateDto, User>();
        CreateMap<UserUpdateDto, User>();
        
        CreateMap<CourtOwner, CourtOwnerViewDto>().ReverseMap();
        CreateMap<CourtOwnerUpdateDto, CourtOwner>();
        
    }
}

public class Contract
{
    public string CourtOwner = "CourtOwner";
    public string User = "User";
}