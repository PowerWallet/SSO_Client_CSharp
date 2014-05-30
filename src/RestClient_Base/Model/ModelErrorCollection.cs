using System;
using System.Collections.ObjectModel;

namespace FinApps.SSO.RestClient_Base.Model
{
    [Serializable]
    public class ModelErrorCollection : Collection<ApiModelError>
    {
        public void Add(Exception exception)
        {
            Add(new ApiModelError(exception));
        }

        public void Add(string errorMessage)
        {
            Add(new ApiModelError(errorMessage));
        }
    }
}