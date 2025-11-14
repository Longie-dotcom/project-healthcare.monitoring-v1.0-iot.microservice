using Application.GrpcService;
using Application.Interface.IService;
using Application.Mapper;
using Application.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ApplicationModule
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => { }, typeof(EdgeDeviceMappingProfile).Assembly);

            services.AddScoped<IEdgeDeviceService, EdgeDeviceService>();

            // gRPC Service
            services.AddScoped<EdgeDeviceGrpcService>();
            return services;
        }
        #endregion
    }
}
