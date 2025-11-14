using HCM.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace HCM.Authorization.Authorization
{
    public class DynamicPrivilegePolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public DynamicPrivilegePolicyProvider(IOptions<AuthorizationOptions> options)
            : base(options)
        {
        }

        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // Try to get an existing policy first
            var existingPolicy = await base.GetPolicyAsync(policyName);
            if (existingPolicy != null)
                return existingPolicy;

            // If it doesn’t exist, create one dynamically
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PrivilegeRequirement(policyName))
                .Build();

            return policy;
        }
    }
}
