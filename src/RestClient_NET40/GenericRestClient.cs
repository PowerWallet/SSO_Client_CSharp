using System;
using System.Globalization;
using System.Net;
using System.Text;
using RestSharp;
using RestSharp.Extensions;

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

            //request.OnBeforeDeserialization = resp =>
            //{
            //    if (((int) resp.StatusCode) < 400) 
            //        return;

            //    // have to read the bytes so .Content doesn't get populated
            //    const string restException = "{{ \"ApiModelState\" : {0} }}";
            //    var content = resp.RawBytes.AsString(); //get the response content
            //    var newJson = string.Format(restException, content);
            //    resp.Content = null;
            //    resp.RawBytes = Encoding.UTF8.GetBytes(newJson.ToString(CultureInfo.InvariantCulture));
            //};

            var response = client.Execute<T>(request);
            if (response.ErrorException == null)
                return response.Data;

            var exception = new ApplicationException("Error retrieving response.  Check inner details for more info.", response.ErrorException);
            throw exception;
        }
    }
}