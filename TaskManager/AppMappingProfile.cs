using AutoMapper;
using TaskManager.Models;
using TaskManagerProvider;

public class AppMappingProfile : Profile
{
    public AppMappingProfile()
    {
        CreateMap<UserItem, User>();

        CreateMap<User, UserItem>();

        CreateMap<TaskManagerProvider.TaskItem, TaskManager.Models.Task>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

        CreateMap<TaskManager.Models.Task, TaskManagerProvider.TaskItem>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
    }
}