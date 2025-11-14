using Application.GrpcService;
using Application.Interface.IService;
using Application.Mapper;
using Application.Service;
using Infrastructure.Service;
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
            services.AddAutoMapper(cfg => { }, typeof(UserMappingProfile).Assembly);
            services.AddAutoMapper(cfg => { }, typeof(RoleMappingProfile).Assembly);
            services.AddAutoMapper(cfg => { }, typeof(PrivilegeMappingProfile).Assembly);

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPrivilegeService, PrivilegeService>();

            // gRPC Service
            services.AddScoped<IAMGrpcService>();
            return services;

        }
        #endregion
    }
}
