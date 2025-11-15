using Application.DTO;
using AutoMapper;
using Domain.Aggregate;
using Domain.Entity;
using Domain.ValueObject;

namespace Application.Mapper
{
    public class RoomProfileMappingProfile : Profile
    {
        public RoomProfileMappingProfile()
        {
            // RoomProfile -> RoomProfileDTO
            CreateMap<RoomProfile, RoomProfileDTO>()
                .ForMember(dest => dest.DeviceProfiles, opt => opt.MapFrom(src => src.DeviceProfiles));

            // DeviceProfile -> DeviceProfileDTO
            CreateMap<DeviceProfile, DeviceProfileDTO>()
                .ForMember(dest => dest.Sensors, opt => opt.MapFrom(src => src.Sensors))
                .ForMember(dest => dest.PatientStaffs, opt => opt.MapFrom(src => src.PatientStaffs));

            // PatientSensor -> PatientSensorDTO
            CreateMap<PatientSensor, PatientSensorDTO>()
                .ForMember(dest => dest.SensorDatas, opt => opt.MapFrom(src => src.SensorDatas));

            // SensorData -> SensorDataDTO
            CreateMap<SensorData, SensorDataDTO>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value.ToString()));

            // PatientStaff -> PatientStaffDTO
            CreateMap<PatientStaff, PatientStaffDTO>();
        }
    }
}
