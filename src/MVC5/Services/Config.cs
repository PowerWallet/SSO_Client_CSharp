using System;
using System.Configuration;

namespace FinApps.SSO.MVC5.Services
{
    public interface IConfig
    {
        string Get(string key);
    }

    public class Config : IConfig
    {
        public string Get(string key)
        {
            string fromConfig = ConfigurationManager.AppSettings[key];

            return String.Equals(fromConfig, "{ENV}", StringComparison.InvariantCultureIgnoreCase)
                ? Environment.GetEnvironmentVariable(key)
                : fromConfig;
        }
    }
}