using System.Threading.Tasks;
using FinApps.SSO.RestClient_Base.Annotations;
using FinApps.SSO.RestClient_Base.Model;

namespace FinApps.SSO.RestClient_NET451
{
    public interface IFinAppsRestClient
    {
        /// <summary>
        /// Creates a new user on FinApps application. If succesful, the response includes a UserToken that uniquely identifies the user. 
        /// </summary>
        /// <param name="finAppsUser">The Financial Apps user.</param>
        /// <returns></returns>
        [UsedImplicitly]
        Task<FinAppsUser> NewUser(FinAppsUser finAppsUser);

        /// <summary>
        /// Starts a new session on FinApps application. If succesful, a one time use SessionToken will be generated.
        /// </summary>
        /// <param name="finAppsCredentials">The fin apps credentials.</param>
        /// <param name="clientIp">The client ip.</param>
        /// <returns></returns>
        [UsedImplicitly]
        Task<FinAppsUser> NewSession(FinAppsCredentials finAppsCredentials, string clientIp);

        /// <summary>
        /// Updates the user profile. Only First Name, Last Name, Email and Postal Code are updated. 
        /// Password s updated through the UpdatePassword call.
        /// If Email changes, a new UserToken will be generated.
        /// </summary>
        /// <param name="finAppsCredentials">The fin apps credentials.</param>
        /// <param name="finAppsUser">The fin apps user.</param>
        /// <returns></returns>
        [UsedImplicitly]
        Task<FinAppsUser> UpdateUserProfile(FinAppsCredentials finAppsCredentials, FinAppsUser finAppsUser);

        /// <summary>
        /// Updates the user password. If succesful, a new UserToken will be generated.
        /// </summary>
        /// <param name="finAppsCredentials">The fin apps credentials.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns></returns>
        [UsedImplicitly]
        Task<FinAppsUser> UpdateUserPassword(FinAppsCredentials finAppsCredentials, string oldPassword,
            string newPassword);

        /// <summary>
        /// Deletes the user from FinApps.
        /// </summary>
        /// <param name="finAppsCredentials">The fin apps credentials.</param>
        /// <returns></returns>
        [UsedImplicitly]
        Task<FinAppsUser> DeleteUser(FinAppsCredentials finAppsCredentials);
    }
}