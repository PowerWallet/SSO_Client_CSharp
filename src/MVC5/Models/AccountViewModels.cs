using System.ComponentModel.DataAnnotations;
using FinApps.SSO.RestClient_Base.Model;

namespace FinApps.SSO.MVC5.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]
        [DataType(DataType.PostalCode)]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
    }

    public static class RegisterViewModelExtensions
    {
        public static ApplicationUser ToApplicationUser(this RegisterViewModel model, string finAppsUserToken = null)
        {
            return new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.UserName,
                PostalCode = model.PostalCode,
                FirstName = model.FirstName,
                LastName = model.LastName,
                FinAppsUserToken = finAppsUserToken
            };
        }

        public static FinAppsUser ToFinAppsUser(this RegisterViewModel model)
        {
            return new FinAppsUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.UserName,
                Password = model.Password,
                PostalCode = model.PostalCode
            };
        }
    }

    public class UpdateProfileViewModel
    {
        public UpdateProfileViewModel(ApplicationUser user)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            UserName = user.UserName;
            PostalCode = user.PostalCode;
        }

        public UpdateProfileViewModel()
        {            
        }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]
        [DataType(DataType.PostalCode)]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
    }

    public static class UpdateProfileViewModelExtensions
    {
        public static FinAppsUser ToFinAppsUser(this UpdateProfileViewModel model)
        {
            var finAppsUser = new FinAppsUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.UserName,
                PostalCode = model.PostalCode
            };
            return finAppsUser;            
        }
    }
}
