using System.Collections.Generic;

namespace FinApps.SSO.RestClient_Base.Model
{
    public abstract class FinAppsBase
    {
        public List<FinAppsError> Errors { get; set; }
    }

    public class FinAppsError
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
    }
}