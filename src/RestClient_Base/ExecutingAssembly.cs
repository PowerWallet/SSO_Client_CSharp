using System;
using System.Reflection;
using FinApps.SSO.RestClient_Base.Annotations;

namespace FinApps.SSO.RestClient_Base
{
    [UsedImplicitly]
    public static class ExecutingAssembly
    {
        // ReSharper disable UnusedMember.Global
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