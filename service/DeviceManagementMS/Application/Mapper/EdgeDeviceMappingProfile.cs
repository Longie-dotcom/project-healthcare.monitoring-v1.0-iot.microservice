using Application.DTO;
using Domain.Aggregate;
using Domain.Entity;
using AutoMapper;

namespace Application.Mapper
{
    public class EdgeDeviceMappingProfile : Profile
    {
        public EdgeDeviceMappingProfile()
        {
            // -------------------------------
            // Aggregate root
            // -------------------------------
            CreateMap<EdgeDevice, EdgeDeviceDTO>()
                .ForMember(dest => dest.EdgeDeviceID, opt => opt.MapFrom(src => src.EdgeDeviceID))
                .ForMember(dest => dest.EdgeKey, opt => opt.MapFrom(src => src.EdgeKey))
                .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.RoomName))
                .ForMember(dest => dest.IpAddress, opt => opt.MapFrom(src => src.IpAddress))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.Controllers, opt => opt.MapFrom(src => src.Controllers));

            // -------------------------------
            // Child entities
            // -------------------------------
            CreateMap<Controller, ControllerDTO>()
                .ForMember(dest => dest.ControllerID, opt => opt.MapFrom(src => src.ControllerID))
                .ForMember(dest => dest.ControllerKey, opt => opt.MapFrom(src => src.ControllerKey))
                .ForMember(dest => dest.EdgeKey, opt => opt.MapFrom(src => src.EdgeKey))
                .ForMember(dest => dest.BedNumber, opt => opt.MapFrom(src => src.BedNumber))
                .ForMember(dest => dest.IpAddress, opt => opt.MapFrom(src => src.IpAddress))
                .ForMember(dest => dest.FirmwareVersion, opt => opt.MapFrom(src => src.FirmwareVersion))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.Sensors, opt => opt.MapFrom(src => src.Sensors));

            CreateMap<Sensor, SensorDTO>()
                .ForMember(dest => dest.SensorID, opt => opt.MapFrom(src => src.SensorID))
                .ForMember(dest => dest.SensorKey, opt => opt.MapFrom(src => src.SensorKey))
                .ForMember(dest => dest.ControllerKey, opt => opt.MapFrom(src => src.ControllerKey))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
        }
    }
}
