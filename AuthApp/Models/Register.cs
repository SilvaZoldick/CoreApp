using System.ComponentModel.DataAnnotations;

namespace AuthApp.Models
{
    public class Register
    {
        [Required(ErrorMessage = "The field user name is required")]
        [Display(Name= "User Name")]
        public string UserName { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]        
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Password is required")]
        [Compare("Password", ErrorMessage = "The confirm password is not the same")]
        public string ConfirmPassword { get; set; }
    }
}