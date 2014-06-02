using System;
using System.Web.Mvc;
using NLog;

namespace FinApps.SSO.MVC5.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class LogAttribute : ActionFilterAttribute
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var rd = filterContext.RequestContext.RouteData;
            string currentAction = rd.GetRequiredString("action");
            string currentController = rd.GetRequiredString("controller");

            logger.Debug("{0}.{1} => Executing", currentController, currentAction);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var rd = filterContext.RequestContext.RouteData;
            string currentAction = rd.GetRequiredString("action");
            string currentController = rd.GetRequiredString("controller");

            logger.Debug("{0}.{1} => Executed", currentController, currentAction);
        }
    }
}