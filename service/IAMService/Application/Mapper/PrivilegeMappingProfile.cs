using Application.DTO;
using AutoMapper;
using Domain.Aggregate;
using Domain.Entity;

namespace Application.Mapper
{
    public class PrivilegeMappingProfile : Profile
    {
        public PrivilegeMappingProfile()
        {
            CreateMap<Privilege, PrivilegeDTO>()
                .ForMember(dest => dest.PrivilegeID, opt => opt.MapFrom(src => src.PrivilegeID))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
        }
    }
}
