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
            services.AddAutoMapper(cfg => { }, typeof(PatientMappingProfile).Assembly);
            services.AddAutoMapper(cfg => { }, typeof(PatientStatusMappingProfile).Assembly);

            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IPatientStatusService, PatientStatusService>();

            // gRPC Service
            services.AddScoped<IAMGrpcService>();
            return services;

        }
        #endregion
    }
}
