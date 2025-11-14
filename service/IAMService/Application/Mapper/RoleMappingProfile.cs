using Application.DTO;
using AutoMapper;
using Domain.Aggregate;
using Domain.Entity;

namespace Application.Mapper
{
    public class RoleMappingProfile : Profile
    {
        public RoleMappingProfile()
        {
            CreateMap<Role, RoleDTO>()
                .ForMember(dest => dest.RoleID, opt => opt.MapFrom(src => src.RoleID))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.RoleCode, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.PrivilegeID, opt => opt.Ignore());

            CreateMap<Role, RoleDetailDTO>()
                .ForMember(dest => dest.RoleID, opt => opt.MapFrom(src => src.RoleID))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.RoleCode, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Privileges, opt => opt.Ignore());
        }
    }
}
