using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using FinApps.SSO.RestClient.Annotations;
using Newtonsoft.Json;

namespace FinApps.SSO.RestClient
{
    public interface IFinAppsRestClient
    {
        [UsedImplicitly]
        string CompanyIdentifier { get; set; }

        [UsedImplicitly]
        string CompanyToken { get; set; }

        [UsedImplicitly]
        Task<ServiceResult> NewUser(FinAppsUser finAppsUser);

        [UsedImplicitly]
        Task<ServiceResult> NewSession(FinAppsCredentials finAppsCredentials, string clientIp);
    }

    [UsedImplicitly]
    public class FinAppsRestClient : IFinAppsRestClient
    {
        private readonly HttpClient _httpClient;

        public FinAppsRestClient(string baseUrl, string companyIdentifier, string companyToken)
        {
            const string apiVersion = "1.0.0";

            CompanyIdentifier = companyIdentifier;
            CompanyToken = companyToken;

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(string.Format("{0}{1}", baseUrl, apiVersion))
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.Timeout = TimeSpan.FromSeconds(60.0);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-charset", "utf-8");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", UserAgent);
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-FinApps-Token", FinAppsToken);
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

        [UsedImplicitly]
        public string CompanyIdentifier { get; set; }

        [UsedImplicitly]
        public string CompanyToken { get; set; }


        [UsedImplicitly]
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

            return await PostAsync(postData, "users/new");
        }

        [UsedImplicitly]
        public async Task<ServiceResult> NewSession(FinAppsCredentials finAppsCredentials, string clientIp)
        {
            var parameter = string.Format("{0}:{1}", finAppsCredentials.Email, finAppsCredentials.FinAppsUserToken);
            var base64Parameter = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(parameter));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Parameter);

            var postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("ClientIp", clientIp),
            };

            return await PostAsync(postData, "users/login");
        }

        private async Task<ServiceResult> PostAsync(IEnumerable<KeyValuePair<string, string>> postData, string resource)
        {
            try
            {
                HttpContent content = new FormUrlEncodedContent(postData);
                HttpResponseMessage response = await _httpClient.PostAsync(requestUri: resource, content: content);
                if (!response.IsSuccessStatusCode)
                    return new ServiceResult
                    {
                        Result = ResultCodeTypes.EXCEPTION_UnableToConnect,
                        ResultString = string.Format("Error occurred, the status code is {0}.", response.StatusCode)
                    };

                string result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ServiceResult>(result);
            }
            catch (WebException ex)
            {
                return new ServiceResult
                {
                    Result = ResultCodeTypes.EXCEPTION_WebServiceException,
                    ResultString = ex.Message
                };
            }

            catch (TaskCanceledException ex)
            {
                return new ServiceResult
                {
                    Result = ResultCodeTypes.EXCEPTION_WebServiceException,
                    ResultString = ex.Message
                };
            }
        }
    }
}