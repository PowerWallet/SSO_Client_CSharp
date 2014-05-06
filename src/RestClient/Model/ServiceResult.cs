using FinApps.SSO.RestClient.Annotations;
using FinApps.SSO.RestClient.Enum;
using Newtonsoft.Json;

namespace FinApps.SSO.RestClient.Model
{
    [UsedImplicitly]
    public class ServiceResult
    {
        public ResultCodeTypes Result { get; set; }

        public string ResultString { get; set; }

        public object ResultObject { get; set; }
    }

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