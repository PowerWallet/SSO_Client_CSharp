using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using FinApps.SSO.RestClient_Base.Annotations;
using FinApps.SSO.RestClient_Base.Enums;
using FinApps.SSO.RestClient_Base.Model;
using Newtonsoft.Json;

namespace FinApps.SSO.RestClient_NET451
{
    [UsedImplicitly]
    public class FinAppsRestClient : IFinAppsRestClient
    {
        #region private members and constructor

        private const string ApiVersion = "1";

        public FinAppsRestClient(string baseUrl, string companyIdentifier, string companyToken)
        {
            BaseUrl = baseUrl;
            CompanyIdentifier = companyIdentifier;
            CompanyToken = companyToken;
        }

        private string FinAppsToken
        {
            get { return string.Format("{0}:{1}", CompanyIdentifier, CompanyToken); }
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
                Version version = assemblyName.Version;
                return version;
            }
        }

        private string CompanyIdentifier { get; set; }

        private string CompanyToken { get; set; }

        private string BaseUrl { get; set; }

        private async Task<ServiceResult> SendAsync(string requestType,
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
                            response =
                                await
                                    httpClient.PostAsync(requestUri: resource,
                                        content: new FormUrlEncodedContent(postData));
                            break;
                        case "PUT":
                            response =
                                await
                                    httpClient.PutAsync(requestUri: resource,
                                        content: new FormUrlEncodedContent(postData));
                            break;
                        case "DELETE":
                            response = await httpClient.DeleteAsync(requestUri: resource);
                            break;
                    }

                    if (response == null)
                    {
                        return null;
                    }

                    if (!response.IsSuccessStatusCode)
                        return UnableToConnectServiceResult(response);

                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ServiceResult>(result);
                }
                catch (WebException ex)
                {
                    return ExceptionServiceResult(ex);
                }
                catch (TaskCanceledException ex)
                {
                    return ExceptionServiceResult(ex);
                }
            }
        }

        private async Task<ServiceResult> PostAsync(IEnumerable<KeyValuePair<string, string>> postData, string resource)
        {
            return await PostAsync(postData, resource, null);
        }

        private async Task<ServiceResult> PostAsync(IEnumerable<KeyValuePair<string, string>> postData, string resource,
            AuthenticationHeaderValue authenticationHeaderValue)
        {
            return await SendAsync("POST", postData, resource, authenticationHeaderValue);
        }

        private async Task<ServiceResult> PutAsync(IEnumerable<KeyValuePair<string, string>> postData, string resource,
            AuthenticationHeaderValue authenticationHeaderValue)
        {
            return await SendAsync("PUT", postData, resource, authenticationHeaderValue);
        }

        private async Task<ServiceResult> DeleteAsync(string resource,
            AuthenticationHeaderValue authenticationHeaderValue)
        {
            return await SendAsync("DELETE", null, resource, authenticationHeaderValue);
        }

        private static ServiceResult ExceptionServiceResult(Exception ex)
        {
            return new ServiceResult
            {
                Result = ResultCodeTypes.EXCEPTION_WebServiceException,
                ResultString = ex.Message
            };
        }

        private static ServiceResult UnableToConnectServiceResult(HttpResponseMessage response = null)
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
                BaseAddress = new Uri(string.Format("{0}v{1}/", BaseUrl, ApiVersion))
            };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.Timeout = TimeSpan.FromSeconds(60.0);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-charset", "utf-8");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", UserAgent);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-FinApps-Token", FinAppsToken);

            return httpClient;
        }

        private HttpClient InitializeHttpClient(AuthenticationHeaderValue authenticationHeaderValue)
        {
            var httpClient = InitializeHttpClient();

            if (authenticationHeaderValue != null)
                httpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

            return httpClient;
        }

        private static AuthenticationHeaderValue BuildAuthenticationHeaderValue(FinAppsCredentials finAppsCredentials)
        {
            string parameter = string.Format("{0}:{1}", finAppsCredentials.Email, finAppsCredentials.FinAppsUserToken);
            string base64Parameter = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(parameter));
            var authenticationHeaderValue = new AuthenticationHeaderValue("Basic", base64Parameter);
            return authenticationHeaderValue;
        }

        #endregion

        public async Task<ServiceResult> NewUser(FinAppsUser finAppsUser)
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

        public async Task<string> NewSession(FinAppsCredentials finAppsCredentials, string clientIp)
        {
            var postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("ClientIp", clientIp),
            };

            var authenticationHeaderValue = BuildAuthenticationHeaderValue(finAppsCredentials);

            ServiceResult serviceResult = await PostAsync(postData, "users/Login", authenticationHeaderValue);
            if (serviceResult == null || serviceResult.Result != ResultCodeTypes.SUCCESSFUL)
                return null;

            return serviceResult.GetRedirectUrl();
        }

        public async Task<ServiceResult> UpdateUserProfile(FinAppsCredentials finAppsCredentials,
            FinAppsUser finAppsUser)
        {
            var postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("FirstName", finAppsUser.FirstName),
                new KeyValuePair<string, string>("LastName", finAppsUser.LastName),
                new KeyValuePair<string, string>("Email", finAppsUser.Email),
                new KeyValuePair<string, string>("PostalCode", finAppsUser.PostalCode)
            };

            var authenticationHeaderValue = BuildAuthenticationHeaderValue(finAppsCredentials);
            return await PutAsync(postData, "users/Update", authenticationHeaderValue);
        }

        public async Task<ServiceResult> UpdateUserPassword(FinAppsCredentials finAppsCredentials, string oldPassword,
            string newPassword)
        {
            var postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("OldPassword", oldPassword),
                new KeyValuePair<string, string>("NewPassword", newPassword)
            };

            var authenticationHeaderValue = BuildAuthenticationHeaderValue(finAppsCredentials);
            return await PutAsync(postData, "users/UpdatePassword", authenticationHeaderValue);
        }

        public async Task<ServiceResult> DeleteUser(FinAppsCredentials finAppsCredentials)
        {
            var authenticationHeaderValue = BuildAuthenticationHeaderValue(finAppsCredentials);
            return await DeleteAsync("users/Delete", authenticationHeaderValue);
        }
    }
}