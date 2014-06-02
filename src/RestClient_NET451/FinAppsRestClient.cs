using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using FinApps.SSO.RestClient_Base;
using FinApps.SSO.RestClient_Base.Annotations;
using FinApps.SSO.RestClient_Base.Model;
using Newtonsoft.Json;

namespace FinApps.SSO.RestClient_NET451
{
    [UsedImplicitly]
    public class FinAppsRestClient : IFinAppsRestClient
    {
        #region private members and constructor

        private const string ApiVersion = "1";
        private static string _finAppsToken;
        private static string _baseUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="FinAppsRestClient"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL for the API.</param>
        /// <param name="companyIdentifier">The company identifier.</param>
        /// <param name="companyToken">The company token.</param>
        public FinAppsRestClient(string baseUrl, string companyIdentifier, string companyToken)
        {
            _baseUrl = baseUrl;
            _finAppsToken = string.Format("{0}:{1}", companyIdentifier, companyToken);
        }

        private static string UserAgent
        {
            get { return string.Format("finapps-csharp/{0} (.NET {1})", ExecutingAssembly.AssemblyVersion, Environment.Version); }
        }

        private static AuthenticationHeaderValue AuthenticationHeaderValue { get; set; }

        private static void SetAuthenticationHeaderValue(FinAppsCredentials credentials)
        {
            AuthenticationHeaderValue = credentials != null
                ? new AuthenticationHeaderValue("Basic", credentials.To64BaseEncodedCredentials())
                : null;
        }

        /// <summary>
        /// Sends an HTTP request as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestType">Type of the request.</param>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="postData">The post data.</param>
        /// <param name="cancellationTokenSource">The cancellation token source.</param>
        /// <returns></returns>
        private static async Task<T> Send<T>(string requestType, string requestUri,
            IEnumerable<KeyValuePair<string, string>> postData = null,
            CancellationTokenSource cancellationTokenSource = null)
            where T : FinAppsBase, new()
        {
            var t = new T();

            try
            {
                HttpResponseMessage response = await SendHttpRequest(requestType, requestUri, postData, cancellationTokenSource);
                if (response != null && response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(result);
                }

                AddResponseError(t, response);
            }
            catch (WebException ex)
            {
                t.Errors.Add(new FinAppsError { PropertyName = string.Empty, ErrorMessage = ex.Message });
            }
            catch (TaskCanceledException ex)
            {
                if (cancellationTokenSource != null && ex.CancellationToken == cancellationTokenSource.Token)
                {
                    // Todo: Handle cancellation, triggered by the caller.
                }
                t.Errors.Add(new FinAppsError { PropertyName = string.Empty, ErrorMessage = ex.Message });
            }

            return t;
        }

        private static void AddResponseError<T>(T t, HttpResponseMessage response = null) where T : FinAppsBase, new()
        {
            t.Errors.Add(new FinAppsError
            {
                PropertyName = string.Empty,
                ErrorMessage = response != null
                    ? string.Format("Unexpected error connecting to API server. Status Code: {0}.", response.StatusCode)
                    : "Unexpected error connecting to API server."
            });
        }
        
        private static async Task<HttpResponseMessage> SendHttpRequest(string requestType, string requestUri,
            IEnumerable<KeyValuePair<string, string>> postData = null,
            CancellationTokenSource cancellationTokenSource = null)
        {
            HttpResponseMessage response = null;

            var urlEncodedContent = new FormUrlEncodedContent(postData);
            CancellationToken cancellationToken = cancellationTokenSource == null
                ? CancellationToken.None
                : cancellationTokenSource.Token;

            using (HttpClient httpClient = InitializeHttpClient())
            {
                switch (requestType)
                {
                    case "POST":
                        response = await httpClient.PostAsync(requestUri, urlEncodedContent, cancellationToken);
                        break;
                    case "PUT":
                        response = await httpClient.PutAsync(requestUri, urlEncodedContent, cancellationToken);
                        break;
                    case "DELETE":
                        response = await httpClient.DeleteAsync(requestUri, cancellationToken);
                        break;
                }
            }
            return response;
        }

        private static HttpClient InitializeHttpClient()
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
            if (_finAppsToken != null)
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-FinApps-Token", _finAppsToken);
            if (AuthenticationHeaderValue != null)
                httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue;
            
            return httpClient;
        }

        #endregion

        /// <summary>
        /// Creates a new user on FinApps application. If succesful, the response includes a UserToken that uniquely identifies the user. 
        /// </summary>
        /// <param name="finAppsUser">The Financial Apps user.</param>
        /// <returns></returns>
        public async Task<FinAppsUser> NewUser(FinAppsUser finAppsUser)
        {
            var postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("FirstName", finAppsUser.FirstName),
                new KeyValuePair<string, string>("LastName", finAppsUser.LastName),
                new KeyValuePair<string, string>("Email", finAppsUser.Email),
                new KeyValuePair<string, string>("Password", finAppsUser.Password),
                new KeyValuePair<string, string>("PostalCode", finAppsUser.PostalCode)
            };

            SetAuthenticationHeaderValue(null);
            return await Send<FinAppsUser>("POST", ApiUris.NewUser, postData);
        }

        /// <summary>
        /// Starts a new session on FinApps application. If succesful, a one time use SessionToken will be generated.
        /// </summary>
        /// <param name="finAppsCredentials">The fin apps credentials.</param>
        /// <param name="clientIp">The client ip.</param>
        /// <returns></returns>
        public async Task<FinAppsUser> NewSession(FinAppsCredentials finAppsCredentials, 
            string clientIp)
        {
            var postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("ClientIp", clientIp),
            };

            SetAuthenticationHeaderValue(finAppsCredentials);
            return await Send<FinAppsUser>("POST", ApiUris.NewSession, postData);
        }

        /// <summary>
        /// Updates the user profile. Only First Name, Last Name, Email and Postal Code are updated. 
        /// Password s updated through the UpdatePassword call.
        /// If Email changes, a new UserToken will be generated.
        /// </summary>
        /// <param name="finAppsCredentials">The fin apps credentials.</param>
        /// <param name="finAppsUser">The fin apps user.</param>
        /// <returns></returns>
        public async Task<FinAppsUser> UpdateUserProfile(FinAppsCredentials finAppsCredentials,
            FinAppsUser finAppsUser)
        {
            var postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("FirstName", finAppsUser.FirstName),
                new KeyValuePair<string, string>("LastName", finAppsUser.LastName),
                new KeyValuePair<string, string>("Email", finAppsUser.Email),
                new KeyValuePair<string, string>("PostalCode", finAppsUser.PostalCode)
            };

            SetAuthenticationHeaderValue(finAppsCredentials);
            return await Send<FinAppsUser>("PUT", ApiUris.UpdateUserProfile, postData);
        }

        /// <summary>
        /// Updates the user password. If succesful, a new UserToken will be generated.
        /// </summary>
        /// <param name="finAppsCredentials">The fin apps credentials.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns></returns>
        public async Task<FinAppsUser> UpdateUserPassword(FinAppsCredentials finAppsCredentials, 
            string oldPassword, string newPassword)
        {
            var postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("OldPassword", oldPassword),
                new KeyValuePair<string, string>("NewPassword", newPassword)
            };

            SetAuthenticationHeaderValue(finAppsCredentials);
            return await Send<FinAppsUser>("PUT", ApiUris.UpdateUserPassword, postData);
        }

        /// <summary>
        /// Deletes the user from FinApps.
        /// </summary>
        /// <param name="finAppsCredentials">The fin apps credentials.</param>
        /// <returns></returns>
        public async Task<FinAppsUser> DeleteUser(FinAppsCredentials finAppsCredentials)
        {
            SetAuthenticationHeaderValue(finAppsCredentials);
            return await Send<FinAppsUser>("DELETE", ApiUris.DeleteUser);
        }
    }
}