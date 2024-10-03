using AutoMapper;
using Entity;
using NotificationManagement.DTOs.NotificationDto;
using NotificationManagement.DTOs.NotificationDto.ViewDto;

namespace NotificationManagement;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<Notification, NotificationViewDto>();

        CreateMap<NotificationCreateDto, Notification>();
        
        CreateMap<NotificationUpdateDto, Notification>();
    }
}