using System.Collections.Generic;

namespace FinApps.SSO.RestClient_Base.Model
{
    public class RestException
    {
        public string Status { get; set; }

        public string Message { get; set; }

        public List<KeyValuePair<string, string[]>> Errors { get; set; }
    }
}