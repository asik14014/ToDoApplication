using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ToDoApplication.Models
{
    // Models used as parameters to AccountController actions.

    public class AddExternalLoginBindingModel
    {
        [Required]
        [Display(Name = "External access token")]
        public string ExternalAccessToken { get; set; }
    }

    public class ChangePasswordBindingModel
    {
        [Required]
        //[DataType(DataType.Password)]
        [Display(Name = "current password")]
        public string oldpassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        [Display(Name = "new password")]
        public string newpassword { get; set; }

        //[DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("newpassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string confirmpassword { get; set; }
    }

    public class RegisterBindingModel
    {
        [Required]
        [Display(Name = "email")]
        public string email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        [Display(Name = "password")]
        public string password { get; set; }

        //[DataType(DataType.Password)]
        [Display(Name = "confirm password")]
        [Compare("password", ErrorMessage = "The password and confirmation password do not match.")]
        public string confirmpassword { get; set; }
    }

    public class RegisterExternalBindingModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class RemoveLoginBindingModel
    {
        [Required]
        [Display(Name = "Login provider")]
        public string LoginProvider { get; set; }

        [Required]
        [Display(Name = "Provider key")]
        public string ProviderKey { get; set; }
    }

    public class SetPasswordBindingModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        public string password { get; set; }
    }

    public class SetEmailBindingModel
    {
        [Required]
        public string email { get; set; }
    }

    public class SetPushBindingModel
    {
        public bool turnOn { get; set; }
    }

    public class SetFavoriteBindingModel
    {
        [Required]
        public long id { get; set; }

        [Required]
        public bool isFavorite { get; set; }
    }

    public class LoginBindingModel
    {
        [Required]
        public string email { get; set; }

        //[DataType(DataType.Password)]
        [Required]
        public string password { get; set; }
    }

    public class ResetPasswordBindingModel
    {
        [Required]
        public string email { get; set; }
    }

    public class FindUserBindingModel
    {
        [Required]
        public string email { get; set; }
    }
}
