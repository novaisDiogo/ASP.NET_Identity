using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityDoZero.Security
{
    public class CanEditOnlyOtherAdminRolesAndClaimsHandler :
        AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>//Manipulador de autorização personalizado que permite ou nega acessoa um recurso
    {
        private readonly IHttpContextAccessor contextAccessor; //Pega informação da url via injeção de idepedencia userId

        public CanEditOnlyOtherAdminRolesAndClaimsHandler(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesAndClaimsRequirement requirement)
        {
            if (contextAccessor == null)//url null
            {
                return Task.CompletedTask;//retorna tarefa de acesso não autorizado
            }

            string loggedInAdminId =
            context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;//Pega id do usuario logado

            string adminIdBeingEdited = contextAccessor.HttpContext.Request.Query["userId"];//Pega id passada na url

            if(adminIdBeingEdited == null)
            {
                return Task.CompletedTask;
            }

            //Compara id da url com a do usuario logado se for igual não tem acesso só pode editar de outro usuario
            if (context.User.IsInRole("Admin") &&
            context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") &&
            adminIdBeingEdited.ToLower() != loggedInAdminId.ToLower())
            {
                context.Succeed(requirement);//se as logicas acima forem atendidadas retorna com sucesso
            }

            return Task.CompletedTask;
        }
    }
}
