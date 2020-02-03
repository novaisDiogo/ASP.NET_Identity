using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using IdentityDoZero.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using IdentityDoZero.Models;
using IdentityDoZero.Security;

namespace IdentityDoZero
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            // Criar servico do identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Mudar caracteristicas que a senha precisa requerir
                //options.Password.RequiredLength = 10;
                //options.Password.RequiredUniqueChars = 3;
            }).AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 3;
                options.Password.RequireNonAlphanumeric = false;
            });

            services.AddMvc(options =>
            {
                // Configuração para usar no filtro, que exige que o usuario esteja logado na aplicação
                //Usar AllowAnonymous para liberar no controller
                var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
                //aplica o filtro Global
                options.Filters.Add(new AuthorizeFilter(policy));
            }
            ).AddXmlSerializerFormatters();

            //vai direcionar para a pagina de Acesso Negado
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Administration/AccessDenied");
            });

            //Cria uma politica personalizada
            services.AddAuthorization(options =>
            {
                //Claim Policy
                options.AddPolicy("CreateRolePolicy", policy => policy.RequireClaim("Create Role", "true"));

                options.AddPolicy("ViewEditRolePolicy", policy => policy.RequireClaim("Edit Role"));

                options.AddPolicy("EditRolePolicy", policy =>
                policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));//Chama a customização feita em Security/CanEditOnlyOtherAdminRolesAndClaimsHandler

                ////Esse metodo ele inclui comparações logicas com && e ||
                //options.AddPolicy("EditRolePolicy", policy => policy.RequireAssertion(context => context.User.IsInRole("Admin") && context.User.HasClaim(claim =>
                //claim.Type == "Edit Role" && claim.Value == "true") || context.User.IsInRole("Super Admin")));

                options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role", "true"));//Para Add mais claims basta Add .RequireClaim("")

                //Roles Policy
                options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));//Para Add mais Role .Require("Admin", "User", "Manager")

                ////Outro metodo de se fazer
                //services.AddAuthorization(options =>
                //{
                //    options.AddPolicy("EditRolePolicy", policy =>
                //        policy.RequireAssertion(context => AuthorizeAccess(context)));
                //});
                //private bool AuthorizeAccess(AuthorizationHandlerContext context) //referenciado acima
                //{
                //    return context.User.IsInRole("Admin") &&
                //            context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
                //            context.User.IsInRole("Super Admin");
                //}

            });

            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();//serviço de customização de autorização
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();//Segunda costumização

            services.AddHttpContextAccessor();//serviço IHttpContextAccessor usada na classe CanEdit

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
