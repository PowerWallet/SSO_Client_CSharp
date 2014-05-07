using System.Web.Mvc;
using FinApps.SSO.MVC5.Filters;

namespace FinApps.SSO.MVC5
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LogAttribute());
        }
    }
}
