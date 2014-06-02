using FinApps.SSO.RestClient_Base.Annotations;
using FinApps.SSO.RestClient_Base.Model;

namespace FinApps.SSO.RestClient_NET40
{
    public interface IFinAppsRestClient
    {
        [UsedImplicitly]
        FinAppsUser NewUser(FinAppsUser finAppsUser);

        [UsedImplicitly]
        FinAppsUser NewSession(FinAppsCredentials credentials, string clientIp);

        [UsedImplicitly]
        FinAppsUser UpdateUserProfile(FinAppsCredentials credentials, FinAppsUser finAppsUser);

        [UsedImplicitly]
        FinAppsUser UpdateUserPassword(FinAppsCredentials credentials, string oldPassword, string newPassword);

        [UsedImplicitly]
        FinAppsUser DeleteUser(FinAppsCredentials credentials);
    }
}