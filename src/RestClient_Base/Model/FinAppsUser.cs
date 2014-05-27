using FinApps.SSO.RestClient_Base.Annotations;

namespace FinApps.SSO.RestClient_Base.Model
{
    [UsedImplicitly]
    public class FinAppsUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PostalCode { get; set; }
    }
}