using FinApps.SSO.RestClient_Base.Annotations;
using FinApps.SSO.RestClient_Base.Enums;

namespace FinApps.SSO.RestClient_Base.Model
{
    [UsedImplicitly]
    public class ServiceResult
    {
        public ResultCodeTypes Result { get; set; }

        public string ResultString { get; set; }

        public object ResultObject { get; set; }
    }
}