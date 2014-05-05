using FinApps.SSO.RestClient.Annotations;

namespace FinApps.SSO.MVC5.Models
{
    [UsedImplicitly]
    public class NewSessionResponse
    {
        public string RedirectToUrl { get; [UsedImplicitly] set; }
    }
}