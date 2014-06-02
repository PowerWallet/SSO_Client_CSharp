using System;
using FinApps.SSO.RestClient_Base.Annotations;

namespace FinApps.SSO.RestClient_Base.Model
{
    [UsedImplicitly]
    public class FinAppsCredentials
    {
        public string Email { get; set; }
        public string FinAppsUserToken { get; set; }
    }

    [UsedImplicitly]
    public static class FinAppsCredentialsExtensions
    {
        // ReSharper disable once UnusedMember.Global
        public static string To64BaseEncodedCredentials(this FinAppsCredentials credentials)
        {
            string parameter = string.Format("{0}:{1}", credentials.Email, credentials.FinAppsUserToken);
            return Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(parameter));
        }
    }
}