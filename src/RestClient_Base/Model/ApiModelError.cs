using System;

namespace FinApps.SSO.RestClient_Base.Model
{
    [Serializable]
    public class ApiModelError
    {
        public ApiModelError(Exception exception, string errorMessage = null)
            : this(errorMessage)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            Exception = exception;
        }

        public ApiModelError(string errorMessage)
        {
            ErrorMessage = (errorMessage ?? string.Empty);
        }

        public Exception Exception { get; private set; }

        public string ErrorMessage { get; private set; }
    }
}