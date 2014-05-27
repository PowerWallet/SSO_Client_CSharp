using FinApps.SSO.RestClient_Base.Response;
using Newtonsoft.Json;

namespace FinApps.SSO.RestClient_Base.Model
{
    public static class ServiceResultExtensions
    {
        public static string GetRedirectUrl(this ServiceResult serviceResult)
        {
            var newSessionResponse = JsonConvert.DeserializeObject<NewSessionResponse>(serviceResult.ResultObject.ToString());
            return newSessionResponse!=null 
                ? newSessionResponse.RedirectToUrl 
                : null;
        }

        public static string GetUserToken(this ServiceResult serviceResult)
        {
            var newUserResponse = JsonConvert.DeserializeObject<NewUserResponse>(serviceResult.ResultObject.ToString());
            return newUserResponse != null
                ? newUserResponse.UserToken
                : null;
        }
    }
}