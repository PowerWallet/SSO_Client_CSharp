using System.Collections.Generic;

namespace FinApps.SSO.RestClient_Base.Model
{
    public abstract class FinAppsBase
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public List<FinAppsError> Errors { get; set; }
    }
}