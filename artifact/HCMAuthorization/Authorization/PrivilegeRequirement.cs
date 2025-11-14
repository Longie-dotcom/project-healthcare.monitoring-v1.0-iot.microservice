using Microsoft.AspNetCore.Authorization;

namespace HCM.Authorization
{
    public class PrivilegeRequirement : IAuthorizationRequirement
    {
        public string Privilege { get; }

        public PrivilegeRequirement(string privilege)
        {
            Privilege = privilege;
        }
    }
}
