using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(FinApps.SSO.MVC5.Startup))]
namespace FinApps.SSO.MVC5
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
