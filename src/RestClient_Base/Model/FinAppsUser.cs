using System;
using FinApps.SSO.RestClient_Base.Annotations;

namespace FinApps.SSO.RestClient_Base.Model
{
    [UsedImplicitly]
    public class FinAppsUser : FinAppsBase
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string PostalCode { get; set; }

        public string UserToken { get; set; }

        // ReSharper disable once UnusedMember.Global
        public Guid? SessionToken { get; set; }

        public string SessionRedirectUrl { get; set; }
    }
}