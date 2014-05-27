using FinApps.SSO.RestClient_Base.Annotations;

namespace FinApps.SSO.RestClient_Base.Response
{
    [UsedImplicitly]
    public class NewSessionResponse
    {
        public string RedirectToUrl { get; set; }
    }
}