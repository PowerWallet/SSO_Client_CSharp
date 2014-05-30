using System;
using System.Reflection;

namespace FinApps.SSO.RestClient_NET40
{
    public static class ExecutingAssembly
    {
        public static Version AssemblyVersion
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                var assemblyName = new AssemblyName(assembly.FullName);
                Version version = assemblyName.Version;
                return version;
            }
        }
    }
}