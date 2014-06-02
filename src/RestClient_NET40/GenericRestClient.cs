using System;
using RestSharp;

namespace FinApps.SSO.RestClient_NET40
{
    public class GenericRestClient<T> where T : new()
    {
        private readonly string _baseUrl;

        public GenericRestClient(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public T Execute(IRestRequest request, string username = null, string password = null) 
        {
            var client = new RestClient(baseUrl: _baseUrl);
            if (username != null && password != null)
                client.Authenticator =  new HttpBasicAuthenticator(username, password);

            var response = client.Execute<T>(request);
            if (response.ErrorException == null)
                return response.Data;

            var exception = new ApplicationException("Error retrieving response.  Check inner details for more info.", response.ErrorException);
            throw exception;
        }
    }
}