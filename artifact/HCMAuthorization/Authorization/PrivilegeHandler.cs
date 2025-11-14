using Microsoft.AspNetCore.Authorization;

namespace HCM.Authorization
{
    public class PrivilegeHandler : AuthorizationHandler<PrivilegeRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PrivilegeRequirement requirement)
        {
            if (context.User.HasClaim("Privileges", requirement.Privilege))
            {
                context.Succeed(requirement);
            }
            else
            {
                throw new AuthorizationFailedException(requirement.Privilege);
            }

            return Task.CompletedTask;
        }
    }
}
