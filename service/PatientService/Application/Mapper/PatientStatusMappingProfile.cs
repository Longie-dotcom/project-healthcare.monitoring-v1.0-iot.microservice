using Application.DTO;
using AutoMapper;
using Domain.Aggregate;
using Domain.Entity;

namespace Application.Mapper
{
    public class PatientStatusMappingProfile : Profile
    {
        public PatientStatusMappingProfile()
        {
            CreateMap<PatientStatus, PatientStatusDTO>()
                .ForMember(dest => dest.PatientStatusCode, opt => opt.MapFrom(src => src.PatientStatusCode))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
        }        
    }
}
