using System.Threading.Tasks;
using FinApps.SSO.RestClient.Annotations;
using FinApps.SSO.RestClient.Model;

namespace FinApps.SSO.RestClient
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