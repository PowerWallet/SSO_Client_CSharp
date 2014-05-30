using System;

namespace FinApps.SSO.RestClient_Base.Model
{
    [Serializable]
    public class ApiModelState
    {
        private readonly ModelErrorCollection _errors = new ModelErrorCollection();

        public ValueProviderResult Value { get; set; }

        public ModelErrorCollection Errors
        {
            get { return _errors; }
        }
    }
}