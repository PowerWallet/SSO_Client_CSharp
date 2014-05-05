using FinApps.SSO.RestClient;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FinApps.SSO.MVC5.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PostalCode { get; set; }
        public string FinAppsUserToken { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
    }

    public static class ApplicationUserExtensions
    {
        public static FinAppsCredentials ToFinAppsCredentials(this ApplicationUser applicationUser)
        {
            return new FinAppsCredentials
            {
                Email = applicationUser.UserName,
                FinAppsUserToken = applicationUser.FinAppsUserToken
            };
        }

        public static FinAppsUser ToFinAppsUser(this ApplicationUser applicationUser, string password)
        {
            return new FinAppsUser
            {
                FirstName = applicationUser.FirstName,
                LastName = applicationUser.LastName,
                Email = applicationUser.UserName,
                Password = password,
                PostalCode = applicationUser.PostalCode
            };
        }
    }
}