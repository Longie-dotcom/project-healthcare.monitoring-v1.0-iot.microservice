using HCM.Authorization.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace HCM.Authorization
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddPrivilegeAuthorization(
            this IServiceCollection services,
            IEnumerable<string>? customPrivileges = null)
        {
            services.AddSingleton<IAuthorizationHandler, PrivilegeHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, DynamicPrivilegePolicyProvider>();

            return services;
        }
    }
}
