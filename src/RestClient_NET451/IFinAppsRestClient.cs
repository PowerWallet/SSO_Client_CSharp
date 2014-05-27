using System.Threading.Tasks;
using FinApps.SSO.RestClient_Base.Annotations;
using FinApps.SSO.RestClient_Base.Model;

namespace FinApps.SSO.RestClient_NET451
{
    public interface IFinAppsRestClient
    {
        [UsedImplicitly]
        Task<ServiceResult> NewUser(FinAppsUser finAppsUser);

        [UsedImplicitly]
        Task<string> NewSession(FinAppsCredentials finAppsCredentials, string clientIp);

        [UsedImplicitly]
        Task<ServiceResult> UpdateUserProfile(FinAppsCredentials finAppsCredentials, FinAppsUser finAppsUser);

        [UsedImplicitly]
        Task<ServiceResult> UpdateUserPassword(FinAppsCredentials finAppsCredentials, string oldPassword,
            string newPassword);

        [UsedImplicitly]
        Task<ServiceResult> DeleteUser(FinAppsCredentials finAppsCredentials);
    }
}