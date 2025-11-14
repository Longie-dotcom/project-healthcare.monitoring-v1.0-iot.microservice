using Application.DTO;
using AutoMapper;
using Domain.Aggregate;
using Domain.Entity;

namespace Application.Mapper
{
    public class StaffMappingProfile : Profile
    {
        public StaffMappingProfile()
        {
            // -------------------------------
            // Aggregate root
            // -------------------------------
            CreateMap<Staff, StaffDTO>()
                .ForMember(dest => dest.StaffID, opt => opt.MapFrom(src => src.StaffID))
                .ForMember(dest => dest.StaffCode, opt => opt.MapFrom(src => src.StaffCode))
                .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Specialization))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.ProfessionalTitle, opt => opt.MapFrom(src => src.ProfessionalTitle))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))

                .ForMember(dest => dest.IdentityNumber, opt => opt.MapFrom(src => src.IdentityNumber))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.Dob))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))

                .ForMember(dest => dest.StaffLicenses, opt => opt.MapFrom(src => src.StaffLicenses))
                .ForMember(dest => dest.StaffAssignments, opt => opt.MapFrom(src => src.StaffAssignments))
                .ForMember(dest => dest.StaffExperiences, opt => opt.MapFrom(src => src.StaffExperiences))
                .ForMember(dest => dest.StaffSchedules, opt => opt.MapFrom(src => src.StaffSchedules));

            // -------------------------------
            // Child entities (read-only mapping)
            // -------------------------------
            CreateMap<StaffLicense, StaffLicenseDTO>();
            CreateMap<StaffSchedule, StaffScheduleDTO>();
            CreateMap<StaffAssignment, StaffAssignmentDTO>();
            CreateMap<StaffExperience, StaffExperienceDTO>();
        }
    }
}
