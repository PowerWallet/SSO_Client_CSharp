using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using FinApps.SSO.RestClient_Base.Annotations;
using FinApps.SSO.RestClient_Base.Enums;
using FinApps.SSO.RestClient_Base.Extensions;
using FinApps.SSO.RestClient_Base.Model;
using Newtonsoft.Json;

namespace FinApps.SSO.RestClient_NET451
{
    [UsedImplicitly]
    public class FinAppsRestClient<T> : IFinAppsRestClient<T> where T : new()
    {
        #region private members and constructor

        private const string ApiVersion = "1";
        private readonly string _finAppsToken;
        private readonly string _baseUrl;        

        public FinAppsRestClient(string baseUrl, string companyIdentifier, string companyToken)
        {
            _baseUrl = baseUrl;
            _finAppsToken = string.Format("{0}:{1}", companyIdentifier, companyToken);
        }

        private static string UserAgent
        {
            get { return string.Format("finapps-csharp/{0} (.NET {1})", AssemblyVersion, Environment.Version); }
        }

        private static Version AssemblyVersion
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                var assemblyName = new AssemblyName(assembly.FullName);
                return assemblyName.Version;
            }
        }
        
        private async Task<T> SendAsync(string requestType,
            IEnumerable<KeyValuePair<string, string>> postData,
            string resource,
            AuthenticationHeaderValue authenticationHeaderValue)
        {
            using (HttpClient httpClient = InitializeHttpClient(authenticationHeaderValue))
            {
                try
                {
                    HttpResponseMessage response = null;
                    switch (requestType)
                    {
                        case "POST":
                            response = await httpClient.PostAsync(requestUri: resource, content: new FormUrlEncodedContent(postData));
                            break;
                        case "PUT":
                            response = await httpClient.PutAsync(requestUri: resource, content: new FormUrlEncodedContent(postData));
                            break;
                        case "DELETE":
                            response = await httpClient.DeleteAsync(requestUri: resource);
                            break;
                    }

                    if (response == null || !response.IsSuccessStatusCode)
                    {
                        throw new Exception();
                    }
                    //return UnableToConnectServiceResult(response);

                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(result);
                }
                catch (WebException ex)
                {
                    Errors.Add(string.Empty, ex.Message);
                }
                catch (TaskCanceledException ex)
                {
                    Errors.Add(string.Empty, ex.Message);
                }
            }
            return new T();
        }

        private async Task<T> PostAsync(IEnumerable<KeyValuePair<string, string>> postData, string resource,
            AuthenticationHeaderValue authenticationHeaderValue = null)
        {
            return await SendAsync("POST", postData, resource, authenticationHeaderValue);
        }

        private async Task<T> PutAsync(IEnumerable<KeyValuePair<string, string>> postData, string resource,
            AuthenticationHeaderValue authenticationHeaderValue)
        {
            return await SendAsync("PUT", postData, resource, authenticationHeaderValue);
        }

        private async Task<T> DeleteAsync(string resource,
            AuthenticationHeaderValue authenticationHeaderValue)
        {
            return await SendAsync("DELETE", null, resource, authenticationHeaderValue);
        }

        private static ServiceResult UnableToConnectServiceResult(HttpResponseMessage response)
        {
            var serviceResult = new ServiceResult
            {
                Result = ResultCodeTypes.EXCEPTION_UnableToConnect,
                ResultString = response == null
                    ? string.Format("Invalid request.")
                    : string.Format("Unable to connect. Status code of the HTTP response: {0}.", response.StatusCode)
            };
            return serviceResult;
        }

        private HttpClient InitializeHttpClient()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(string.Format("{0}v{1}/", _baseUrl, ApiVersion))
            };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.Timeout = TimeSpan.FromSeconds(60.0);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-charset", "utf-8");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", UserAgent);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-FinApps-Token", _finAppsToken);

            return httpClient;
        }

        private HttpClient InitializeHttpClient(AuthenticationHeaderValue authenticationHeaderValue)
        {
            var httpClient = InitializeHttpClient();

            if (authenticationHeaderValue != null)
                httpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

            return httpClient;
        }

        #endregion

        public async Task<T> NewUser(FinAppsUser finAppsUser)
        {
            var postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("FirstName", finAppsUser.FirstName),
                new KeyValuePair<string, string>("LastName", finAppsUser.LastName),
                new KeyValuePair<string, string>("Email", finAppsUser.Email),
                new KeyValuePair<string, string>("Password", finAppsUser.Password),
                new KeyValuePair<string, string>("PostalCode", finAppsUser.PostalCode)
            };

            return await PostAsync(postData, "users/New");
        }

        public async Task<string> NewSession(FinAppsCredentials finAppsCredentials, 
            string clientIp)
        {
            var postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("ClientIp", clientIp),
            };

            var authenticationHeaderValue = new AuthenticationHeaderValue("Basic", finAppsCredentials.To64BaseEncodedCredentials());

            T serviceResult = await PostAsync(postData, "users/Login", authenticationHeaderValue);
            //if (serviceResult == null || serviceResult.Result != ResultCodeTypes.SUCCESSFUL)
            var result = serviceResult as ServiceResult;
            if (result == null || result.Result != ResultCodeTypes.SUCCESSFUL)
                return null;

            return result.GetRedirectUrl();
        }

        public async Task<T> UpdateUserProfile(FinAppsCredentials finAppsCredentials,
            FinAppsUser finAppsUser)
        {
            var postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("FirstName", finAppsUser.FirstName),
                new KeyValuePair<string, string>("LastName", finAppsUser.LastName),
                new KeyValuePair<string, string>("Email", finAppsUser.Email),
                new KeyValuePair<string, string>("PostalCode", finAppsUser.PostalCode)
            };

            var authenticationHeaderValue = new AuthenticationHeaderValue("Basic", finAppsCredentials.To64BaseEncodedCredentials());
            return await PutAsync(postData, "users/Update", authenticationHeaderValue);
        }

        public async Task<T> UpdateUserPassword(FinAppsCredentials finAppsCredentials, 
            string oldPassword, string newPassword)
        {
            var postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("OldPassword", oldPassword),
                new KeyValuePair<string, string>("NewPassword", newPassword)
            };

            var authenticationHeaderValue = new AuthenticationHeaderValue("Basic", finAppsCredentials.To64BaseEncodedCredentials());
            return await PutAsync(postData, "users/UpdatePassword", authenticationHeaderValue);
        }

        public async Task<T> DeleteUser(FinAppsCredentials finAppsCredentials)
        {
            var authenticationHeaderValue = new AuthenticationHeaderValue("Basic", finAppsCredentials.To64BaseEncodedCredentials());
            return await DeleteAsync("users/Delete", authenticationHeaderValue);
        }

        public Dictionary<string, string> Errors { get; private set; }  
    }
}