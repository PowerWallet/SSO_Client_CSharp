using System.Web.Mvc;
using FinApps.SSO.MVC4.Filters;

namespace FinApps.SSO.MVC4
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new InitializeSimpleMembershipAttribute());
        }
    }
}