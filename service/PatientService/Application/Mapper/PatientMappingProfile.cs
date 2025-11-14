using Application.DTO;
using AutoMapper;
using Domain.Aggregate;
using Domain.Entity;

namespace Application.Mapper
{
    public class PatientMappingProfile : Profile
    {
        public PatientMappingProfile()
        {
            // -------------------------------
            // Aggregate root
            // -------------------------------
            CreateMap<Patient, PatientDTO>()
                .ForMember(dest => dest.PatientID, opt => opt.MapFrom(src => src.PatientID))
                .ForMember(dest => dest.PatientCode, opt => opt.MapFrom(src => src.PatientCode))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.PatientStatusCode))
                .ForMember(dest => dest.AdmissionDate, opt => opt.MapFrom(src => src.AdmissionDate))
                .ForMember(dest => dest.DischargeDate, opt => opt.MapFrom(src => src.DischargeDate))
                .ForMember(dest => dest.DischargeDate, opt => opt.MapFrom(src => src.DischargeDate))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))

                .ForMember(dest => dest.IdentityNumber, opt => opt.MapFrom(src => src.IdentityNumber))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.Dob))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))

                .ForMember(dest => dest.PatientBedAssignments, opt => opt.MapFrom(src => src.PatientBedAssignments))
                .ForMember(dest => dest.PatientStaffAssignments, opt => opt.MapFrom(src => src.PatientStaffAssignment));

            // -------------------------------
            // Child entities (read-only mapping)
            // -------------------------------
            CreateMap<PatientBedAssignment, PatientBedAssignmentDTO>();
            CreateMap<PatientStaffAssignment, PatientStaffAssignmentDTO>();
        }
    }
}
