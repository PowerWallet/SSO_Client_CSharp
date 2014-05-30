using FinApps.SSO.RestClient_Base.Annotations;
using FinApps.SSO.RestClient_Base.Model;

namespace FinApps.SSO.RestClient_NET40
{
    public interface IFinAppsRestClient
    {
        [UsedImplicitly]
        FinAppsUser NewUser(FinAppsUser finAppsUser);

        [UsedImplicitly]
        ServiceResult NewSession(FinAppsCredentials credentials, string clientIp);

        [UsedImplicitly]
        ServiceResult UpdateUserProfile(FinAppsCredentials credentials, FinAppsUser finAppsUser);

        [UsedImplicitly]
        ServiceResult UpdateUserPassword(FinAppsCredentials credentials, string oldPassword, string newPassword);

        [UsedImplicitly]
        ServiceResult DeleteUser(FinAppsCredentials credentials);
    }
}