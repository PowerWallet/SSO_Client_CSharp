using System;
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
        private readonly GenericRestClient<ServiceResult> _genericRestClient;

        #region private members and constructor

        private const string ApiVersion = "1";
        private readonly string _baseUrl;

        public FinAppsRestClient(string baseUrl, string companyIdentifier, string companyToken)
        {
            CompanyIdentifier = companyIdentifier;
            CompanyToken = companyToken;
            _baseUrl = string.Format("{0}v{1}/", baseUrl, ApiVersion);

            _genericRestClient = new GenericRestClient<ServiceResult>(_baseUrl);
        }

        private static string UserAgent
        {
            get { return string.Format("finapps-csharp/{0} (.NET {1})", ExecutingAssembly.AssemblyVersion, Environment.Version); }
        }

        private static string CompanyIdentifier { get; set; }

        private static string CompanyToken { get; set; }

        private static RestRequest CreateRestRequest(Method method, string resource)
        {
            var request = new RestRequest(method)
            {
                Resource = resource,
                Timeout = TimeSpan.FromSeconds(60.0).Milliseconds
            };
            
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Accept-Encoding", "gzip, deflate");
            request.AddHeader("Accept-charset", "utf-8");
            request.AddHeader("User-Agent", UserAgent);
            request.AddHeader("X-FinApps-Token", string.Format("{0}:{1}", CompanyIdentifier, CompanyToken));

            return request;
        }

        #endregion

        public FinAppsUser NewUser(FinAppsUser finAppsUser)
        {
            Require.Argument("Email", finAppsUser.Email);
            Require.Argument("Password", finAppsUser.Password);
            Require.Argument("PostalCode", finAppsUser.PostalCode);

            RestRequest request = CreateRestRequest(Method.POST, Resources.NewUser);
            request.AddParameter("Email", finAppsUser.Email);
            request.AddParameter("Password", finAppsUser.Password);
            request.AddParameter("PostalCode", finAppsUser.PostalCode);
            if (!string.IsNullOrWhiteSpace(finAppsUser.FirstName)) 
                request.AddParameter("FirstName", finAppsUser.FirstName);
            if (!string.IsNullOrWhiteSpace(finAppsUser.LastName)) 
                request.AddParameter("LastName", finAppsUser.LastName);

            var genericRestClient = new GenericRestClient<FinAppsUser>(_baseUrl);
            return genericRestClient.Execute(request);
        }

        public ServiceResult NewSession(FinAppsCredentials credentials, string clientIp)
        {
            RestRequest request = CreateRestRequest(Method.POST, Resources.NewSession);        
            if (string.IsNullOrWhiteSpace(clientIp))
                request.AddParameter("ClientIp", clientIp);

            return _genericRestClient.Execute(request, credentials.Email, credentials.FinAppsUserToken);
        }

        public ServiceResult UpdateUserProfile(FinAppsCredentials credentials, FinAppsUser finAppsUser)
        {
            RestRequest request = CreateRestRequest(Method.PUT, Resources.UpdateUserProfile);
            return _genericRestClient.Execute(request, credentials.Email, credentials.FinAppsUserToken);
        }

        public ServiceResult UpdateUserPassword(FinAppsCredentials credentials, string oldPassword,
            string newPassword)
        {
            Require.Argument("OldPassword", oldPassword);
            Require.Argument("NewPassword", newPassword);

            RestRequest request = CreateRestRequest(Method.PUT, Resources.UpdateUserPassword);
            request.AddParameter("OldPassword", oldPassword);
            request.AddParameter("NewPassword", newPassword);

            return _genericRestClient.Execute(request, credentials.Email, credentials.FinAppsUserToken);
        }

        public ServiceResult DeleteUser(FinAppsCredentials credentials)
        {
            RestRequest request = CreateRestRequest(Method.DELETE, Resources.DeleteUser);
            return _genericRestClient.Execute(request, credentials.Email, credentials.FinAppsUserToken);
        }
    }
}