using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDoZero.Security
{
    public class SuperAdminHandler :
        AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>//Se conter role super Admin pode acessar tudo
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        ManageAdminRolesAndClaimsRequirement requirement)
        {
            if (context.User.IsInRole("Super Admin"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
