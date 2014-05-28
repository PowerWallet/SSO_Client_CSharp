using System;
using FinApps.SSO.RestClient_Base.Enums;
using FinApps.SSO.RestClient_Base.Model;

namespace FinApps.SSO.RestClient_Base.Extensions
{
    public static class ExceptionExtensions
    {
        public static ServiceResult ToServiceResult(this Exception exception)
        {
            return new ServiceResult
            {
                Result = ResultCodeTypes.EXCEPTION_WebServiceException,
                ResultString = exception.Message
            };
        }
    }
}
