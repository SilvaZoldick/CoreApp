using System;
using System.Threading.Tasks;
using AuthAPI.Models;
using AuthAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace AuthAPI.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("v1/account")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
    
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> LoginAsync([FromBody]User model)
        {            
            if (!ModelState.IsValid)
                return NotFound(new { message = "Usuário ou senha inválidos" });
            
            var token = TokenService.GenerateToken(model);
            model.PasswordHash = "";
            
            await signInManager.PasswordSignInAsync(model.UserName, model.PasswordHash, false, false);

            return new {model, token};
        }
        
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<dynamic>> RegisterAsync([FromBody]User model)
        {    
            try
            {
                var result = await userManager.CreateAsync(model);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(model, isPersistent: false);
                    return Ok();
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return new {message = "Can't register user."};
            }
            catch(Exception)
            {
                return NotFound(new { message = "Cant create user" });
            }
        }

        #region TestAccess
        [HttpGet]
        [Route("anonymous")]
        [AllowAnonymous]
        public string Anonymous() => "Anônimo";

        [HttpGet]
        [Route("authenticated")]
        [Authorize]
        public string Authenticated() => String.Format("Autenticado - {0}", User.Identity.Name);

        [HttpGet]
        [Route("employee")]
        [Authorize(Roles = "employee,manager")]
        public string Employee() => "Funcionário";

        [HttpGet]
        [Route("manager")]
        [Authorize(Roles = "manager")]
        public string Manager() => "Gerente";
        #endregion
    }    
}