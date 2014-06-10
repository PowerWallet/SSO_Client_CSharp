using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using FinApps.SSO.RestClient_Base.Annotations;
using FinApps.SSO.RestClient_Base.Model;
using RestSharp;
using RestSharp.Serializers;

namespace FinApps.SSO.RestClient_NET40
{
    public interface IGenericRestClient
    {
        T Execute<T>(IRestRequest request, string username = null, string password = null) where T : new();
    }

    public class GenericRestClient : IGenericRestClient
    {
        private readonly IRestClient _restClient;

        [UsedImplicitly]
        public GenericRestClient(IRestClient restClient)
        {
            _restClient = restClient;
        }

        public GenericRestClient(string baseUrl)
        {
            _restClient = new RestClient(baseUrl);
        }

        private static byte[] ErrorMessageAsRawBytes(string errorMessage)
        {
            var errors = new
            {
                Errors = new List<FinAppsError>
                {
                    new FinAppsError
                    {
                        PropertyName = string.Empty,
                        ErrorMessage = errorMessage
                    }
                }
            };
            return Encoding.UTF8.GetBytes(new JsonSerializer().Serialize(errors));
        }

        public T Execute<T>(IRestRequest request, string username = null, string password = null) where T : new()
        {
            if (username != null && password != null)
                _restClient.Authenticator = new HttpBasicAuthenticator(username, password);

            request.OnBeforeDeserialization = resp =>
            {
                resp.ContentType = "application/json";

                // transport and other non-HTTP errors
                switch (resp.ResponseStatus)
                {
                    case ResponseStatus.TimedOut:
                    case ResponseStatus.Error:
                        resp.Content = null;
                        resp.RawBytes = ErrorMessageAsRawBytes(resp.ErrorMessage);
                        break;
                    default:
                        var apiStatusCodes = new[]
                        {
                            HttpStatusCode.NotFound,
                            HttpStatusCode.Unauthorized,
                            HttpStatusCode.InternalServerError
                        };
                        if (apiStatusCodes.Contains(resp.StatusCode))
                        {
                            resp.Content = null;
                            resp.RawBytes = ErrorMessageAsRawBytes(resp.StatusDescription);
                        }
                        break;
                }
            };

            IRestResponse<T> response = _restClient.Execute<T>(request);
            if (response.ErrorException == null)
                return response.Data;

            var exception = new ApplicationException("Error retrieving response.  Check inner details for more info.",
                response.ErrorException);
            throw exception;
        }
    }
}