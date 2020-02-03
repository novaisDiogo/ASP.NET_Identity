using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityDoZero.Models;
using IdentityDoZero.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdentityDoZero.Controllers
{
    [Authorize] //Só ira funcionar o controller caso esteja logado
    public class AccountController : Controller
    {
        //Injeção da SignInManager e UserManager para usar as funções do Identity
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        
        //Tela de registro
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        //Valida se existe usuario com esse email
        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if(user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already in use");
            }
        }

        //Metodo post para registrar
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //atribui da viewmodel para o identity user as informações digitadas
                var user = new ApplicationUser {
                    UserName = model.Email,
                    Email = model.Email,
                    City = model.City //Campo personalizado
                };
                //Cria o usuario
                var result = await userManager.CreateAsync(user, model.Password);
                //verifica se teve sucesso
                if (result.Succeeded)
                {
                    //Caso seja o ADM fazendo cadastro redireciona para a lista de usuarios
                    if(signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }

                    //loga
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                //Add erros no modelstate para exibir na view
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }
        
        //Tela Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        //Metodo post para Logar
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //Autentica o usuario com os dados fornecidos da LoginViewModel
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                //verifica se teve sucesso
                if (result.Succeeded)
                {
                    if (Url.IsLocalUrl(returnUrl)) //evita ataques na Url do Sistema
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                //Add erros no modelState para exibir na view
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }

            return View(model);
        }
        
        //Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            //Sair
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
