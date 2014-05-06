using FinApps.SSO.RestClient.Annotations;

namespace FinApps.SSO.RestClient.Model
{
    [UsedImplicitly]
    public class NewSessionResponse
    {
        public string RedirectToUrl { get; [UsedImplicitly] set; }
    }
}