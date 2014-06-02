using FinApps.SSO.RestClient_Base.Annotations;

namespace FinApps.SSO.RestClient_Base.Model
{
    [UsedImplicitly]
    public class FinAppsError
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
    }
}