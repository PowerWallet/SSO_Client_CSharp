using FinApps.SSO.RestClient_Base.Annotations;

namespace FinApps.SSO.RestClient_Base
{
    [UsedImplicitly]
    public static class ApiUris
    {
        // ReSharper disable UnusedMember.Global
        public const string NewUser = "users/new";
        public const string NewSession = "users/login";
        public const string UpdateUserProfile = "users/update";
        public const string UpdateUserPassword = "users/updatepassword";
        public const string DeleteUser = "users/delete";
    }
}
