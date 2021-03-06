﻿using FinApps.SSO.RestClient_Base.Model;
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

        public static void UpdateFromViewModel(this ApplicationUser applicationUser, UpdateProfileViewModel model)
        {
            applicationUser.UserName = model.UserName;
            applicationUser.Email = model.UserName;
            applicationUser.PostalCode = model.PostalCode;
            applicationUser.FirstName = model.FirstName;
            applicationUser.LastName = model.LastName;            
        }
    }
}