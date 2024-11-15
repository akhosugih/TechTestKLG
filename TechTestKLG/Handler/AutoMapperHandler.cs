using AutoMapper;
using TechTestKLG.DTO;
using TechTestKLG.Models;

namespace TechTestKLG.Handler
{
    public class AutoMapperHandler : Profile
    {
        public AutoMapperHandler() 
        {
            CreateMap<UserDTO, Users>();
            CreateMap<ActivityDTO, Activities>();
        }
    }
}
