using System;
using System.Reflection;
using FinApps.SSO.RestClient_Base;
using FinApps.SSO.RestClient_Base.Annotations;
using FinApps.SSO.RestClient_Base.Model;
using RestSharp;
using RestSharp.Validation;

namespace FinApps.SSO.RestClient_NET40
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

        private T Execute<T>(IRestRequest request) where T : new()
        {
            return Execute<T>(request, null);
        }

        private T Execute<T>(IRestRequest request, FinAppsCredentials credentials) where T : new()
        {
            var client = new RestClient(baseUrl: string.Format("{0}v{1}/", BaseUrl, ApiVersion));
            if (credentials != null)
                client.Authenticator = new HttpBasicAuthenticator(credentials.Email, credentials.FinAppsUserToken);

            request.Timeout = TimeSpan.FromSeconds(60.0).Milliseconds;
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Accept-Encoding", "gzip, deflate");
            request.AddHeader("Accept-charset", "utf-8");
            request.AddHeader("User-Agent", UserAgent);
            request.AddHeader("X-FinApps-Token", FinAppsToken);

            var response = client.Execute<T>(request);
            if (response.ErrorException == null)
                return response.Data;

            var exception = new ApplicationException("Error retrieving response.  Check inner details for more info.", response.ErrorException);
            throw exception;
        }

        #endregion

        public ServiceResult NewUser(FinAppsUser finAppsUser)
        {
            Require.Argument("Email", finAppsUser.Email);
            Require.Argument("Password", finAppsUser.Password);
            Require.Argument("PostalCode", finAppsUser.PostalCode);

            var request = new RestRequest(Method.POST)
            {
                Resource = Resources.NewUser
            };

            request.AddParameter("Email", finAppsUser.Email);
            request.AddParameter("Password", finAppsUser.Password);
            request.AddParameter("PostalCode", finAppsUser.PostalCode);

            if (string.IsNullOrWhiteSpace(finAppsUser.FirstName)) 
                request.AddParameter("FirstName", finAppsUser.FirstName);
            if (string.IsNullOrWhiteSpace(finAppsUser.LastName)) 
                request.AddParameter("LastName", finAppsUser.LastName);

            return Execute<ServiceResult>(request);
        }

        public ServiceResult NewSession(FinAppsCredentials finAppsCredentials, string clientIp)
        {
            var request = new RestRequest(Method.POST)
            {
                Resource = Resources.NewSession
            };

            if (string.IsNullOrWhiteSpace(clientIp))
                request.AddParameter("ClientIp", clientIp);


            return Execute<ServiceResult>(request, finAppsCredentials);
        }

        public ServiceResult UpdateUserProfile(FinAppsCredentials finAppsCredentials, FinAppsUser finAppsUser)
        {
            var request = new RestRequest(Method.PUT)
            {
                Resource = Resources.UpdateUserProfile
            };

            return Execute<ServiceResult>(request, finAppsCredentials);
        }

        public ServiceResult UpdateUserPassword(FinAppsCredentials finAppsCredentials, string oldPassword,
            string newPassword)
        {
            Require.Argument("OldPassword", oldPassword);
            Require.Argument("NewPassword", newPassword);
            
            var request = new RestRequest(Method.PUT)
            {
                Resource = Resources.UpdateUserPassword
            };

            request.AddParameter("OldPassword", oldPassword);
            request.AddParameter("NewPassword", newPassword);

            return Execute<ServiceResult>(request, finAppsCredentials);
        }

        public ServiceResult DeleteUser(FinAppsCredentials finAppsCredentials)
        {
            var request = new RestRequest(Method.DELETE)
            {
                Resource = Resources.DeleteUser
            };

            return Execute<ServiceResult>(request, finAppsCredentials);
        }
    }
}