using FinApps.SSO.RestClient.Annotations;

namespace FinApps.SSO.RestClient
{
    [UsedImplicitly]
    public class ServiceResult
    {
        public ResultCodeTypes Result { get; set; }

        public string ResultString { get; set; }

        public object ResultObject { get; set; }
    }
}