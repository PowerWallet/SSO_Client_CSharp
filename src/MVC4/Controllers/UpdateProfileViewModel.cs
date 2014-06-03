using System.ComponentModel.DataAnnotations;
using FinApps.SSO.MVC4.Models;
using FinApps.SSO.RestClient_Base.Model;

namespace FinApps.SSO.MVC4.Controllers
{
    public class UpdateProfileViewModel
    {
        public UpdateProfileViewModel(UserProfile user)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            PostalCode = user.PostalCode;
        }

        // ReSharper disable once UnusedMember.Global
        public UpdateProfileViewModel()
        {
        }

        [Required]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string Email { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]
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
                Email = model.Email,
                PostalCode = model.PostalCode
            };
            return finAppsUser;
        }
    }
}