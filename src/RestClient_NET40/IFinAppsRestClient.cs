using FinApps.SSO.RestClient_Base.Annotations;
using FinApps.SSO.RestClient_Base.Model;

namespace FinApps.SSO.RestClient_NET40
{
    public interface IFinAppsRestClient
    {
        [UsedImplicitly]
        ServiceResult NewUser(FinAppsUser finAppsUser);

        [UsedImplicitly]
        ServiceResult NewSession(FinAppsCredentials finAppsCredentials, string clientIp);

        [UsedImplicitly]
        ServiceResult UpdateUserProfile(FinAppsCredentials finAppsCredentials, FinAppsUser finAppsUser);

        [UsedImplicitly]
        ServiceResult UpdateUserPassword(FinAppsCredentials finAppsCredentials, string oldPassword, string newPassword);

        [UsedImplicitly]
        ServiceResult DeleteUser(FinAppsCredentials finAppsCredentials);
    }
}