using System.Threading.Tasks;
using FinApps.SSO.RestClient_Base.Annotations;
using FinApps.SSO.RestClient_Base.Model;

namespace FinApps.SSO.RestClient_NET451
{
    public interface IFinAppsRestClient<T>
    {
        [UsedImplicitly]
        Task<T> NewUser(FinAppsUser finAppsUser);

        [UsedImplicitly]
        Task<string> NewSession(FinAppsCredentials finAppsCredentials, string clientIp);

        [UsedImplicitly]
        Task<T> UpdateUserProfile(FinAppsCredentials finAppsCredentials, FinAppsUser finAppsUser);

        [UsedImplicitly]
        Task<T> UpdateUserPassword(FinAppsCredentials finAppsCredentials, string oldPassword,
            string newPassword);

        [UsedImplicitly]
        Task<T> DeleteUser(FinAppsCredentials finAppsCredentials);
    }
}