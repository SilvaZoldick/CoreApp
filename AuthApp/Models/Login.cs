using System.ComponentModel.DataAnnotations;

namespace AuthApp.Models
{
    public class Login
    {
        [Required(ErrorMessage = "The field user name is required")]
        [Display(Name= "User Name")]
        public string UserName { get; set; }        
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]        
        public string Password { get; set; }
    }
}