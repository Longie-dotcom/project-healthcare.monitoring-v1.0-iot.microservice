using Application.GrpcService;
using Application.Interface.IService;
using Application.Mapper;
using Application.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
            services.AddAutoMapper(cfg => { }, typeof(StaffMappingProfile).Assembly);

            services.AddScoped<IStaffService, StaffService>();

            // gRPC Service
            services.AddScoped<StaffGrpcService>();
            return services;

        }
        #endregion
    }
}
