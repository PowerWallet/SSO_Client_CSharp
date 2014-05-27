using System;
using System.Reflection;
using FinApps.SSO.RestClient_Base.Annotations;
using FinApps.SSO.RestClient_Base.Model;

namespace FinApps.SSO.RestClient_NET40
{
    [UsedImplicitly]
    public class FinAppsRestClient : IFinAppsRestClient
    {
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

        public ServiceResult NewUser(FinAppsUser finAppsUser)
        {
            throw new System.NotImplementedException();
        }

        public ServiceResult NewSession(FinAppsCredentials finAppsCredentials, string clientIp)
        {
            throw new System.NotImplementedException();
        }

        public ServiceResult UpdateUserProfile(FinAppsCredentials finAppsCredentials, FinAppsUser finAppsUser)
        {
            throw new System.NotImplementedException();
        }

        public ServiceResult UpdateUserPassword(FinAppsCredentials finAppsCredentials, string oldPassword, string newPassword)
        {
            throw new System.NotImplementedException();
        }

        public ServiceResult DeleteUser(FinAppsCredentials finAppsCredentials)
        {
            throw new System.NotImplementedException();
        }
    }
}