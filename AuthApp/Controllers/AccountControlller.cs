using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using AuthApp.Models;
using AuthApp.Services;

using Data.Models;
namespace AuthApp.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ILogger<AccountController> logger;
        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager, ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.Logado = signInManager.IsSignedIn(User);
            return View();
        }

        [HttpPost]        
        public async Task<ActionResult<dynamic>> Login([FromForm]Login model)
        {            
            if (!ModelState.IsValid)
                return NotFound(new { message = "Not found" });
            try
            {
                var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
                if (result.Succeeded)
                {
                    var user = await userManager.FindByNameAsync(model.UserName);

                    string token = TokenService.GenerateToken(user);
                    var headers = HttpContext.Request.Headers;
                    var headerToken = "Bearer " + token;
                    headers.Add("Authorization", headerToken);
                    
                    return RedirectToAction("Index", "Home");
                }
                return new {message = "Invalid user's credentials"};
            }
            catch(Exception ex)
            {
                return new { message = ex.Message};
            }

        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]        
        public async Task<ActionResult<dynamic>> Register([FromForm]Register model)
        {    
            try
            {
                if(!ModelState.IsValid)
                    return NotFound(new { message = "Not found" });

                var user = new User 
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);

                    var token = TokenService.GenerateToken(user);
                    //var refreshToken = TokenService.GenerateRefreshToken();
                    //TokenService.SaveRefreshToken(user.UserName, refreshToken);
                    var headers = HttpContext.Request.Headers;
                    headers.Add("Authorization", "Bearer " + token);

                    return RedirectToAction("Index", "Home");
                }
                string errorList = null;
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    errorList += " | " + error.Description;
                }
                return new {message = "Can't register user. | " + errorList};
            }
            catch(Exception ex)
            {
                return NotFound(new { message = "An error has occurred. Error: " +  ex.Message});
            }
        }

        #region TestAccess
        [HttpGet]
        [Route("anonymous")]
        [AllowAnonymous]
        public IActionResult Anonymous() => View();        

        [HttpGet]
        [Route("authenticated")]
        [Authorize]
        public IActionResult Authenticated()
        {
            ViewBag.AuthenticatedInfo = string.Format("Autenticado - {0}", User.Identity.Name);
            return View();
        }

        [HttpGet]
        [Route("employee")]
        [Authorize(Roles = "employee,manager")]
        public IActionResult Employee() => View();

        [HttpGet]
        [Route("manager")]
        [Authorize(Roles = "manager")]
        public IActionResult Manager() => View();
        #endregion
    }    
}