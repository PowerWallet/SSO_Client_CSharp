using FinApps.SSO.RestClient_Base.Annotations;

namespace FinApps.SSO.RestClient_Base.Model
{
    [UsedImplicitly]
    public class FinAppsCredentials
    {
        public string Email { get; set; }
        public string FinAppsUserToken { get; set; }
    }
}