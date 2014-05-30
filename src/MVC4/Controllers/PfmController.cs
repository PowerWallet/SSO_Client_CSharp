using System;
using System.Linq;
using System.Web.Mvc;
using FinApps.SSO.MVC4.Models;
using FinApps.SSO.RestClient_Base.Annotations;
using FinApps.SSO.RestClient_Base.Model;
using FinApps.SSO.RestClient_NET40;
using NLog;
using Quintsys.EnviromentConfigurationManager;
using WebMatrix.WebData;

namespace FinApps.SSO.MVC4.Controllers
{
    public class PfmController : Controller
    {
        private readonly IFinAppsRestClient _client;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public PfmController(IFinAppsRestClient client)
        {
            _client = client;
        }

        [UsedImplicitly]
        public PfmController()
        {
            IEnviromentConfigManager configuration = new EnviromentConfigManager();
            _client = new FinAppsRestClient(
                baseUrl: configuration.Get("FinAppsDemoUrl"),
                companyIdentifier: configuration.Get("FinAppsCompanyIdentifier"),
                companyToken: configuration.Get("FinAppsCompanyToken"));            
        }

        public ActionResult Index()
        {
            var context = new UsersContext();
            UserProfile user = context.UserProfiles.FirstOrDefault(u => u.Email == User.Identity.Name);
            if (user == null)
            {
                logger.Warn("No valid user found for email: {0}!", User.Identity.Name);

                WebSecurity.Logout();
                return new HttpUnauthorizedResult();
            }

            FinAppsUser finAppsUser = _client.NewSession(user.ToFinAppsCredentials(), Request.UserHostAddress);
            if (finAppsUser.Errors != null)
            {
                foreach (var error in finAppsUser.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View("Error");
            }

            if (string.IsNullOrEmpty(finAppsUser.SessionRedirectUrl))
            {
                logger.Error("Index => Error: Invalid redirect URL.");
                return View("Error");
            }

            Uri absoluteUrl;
            if (!Uri.TryCreate(finAppsUser.SessionRedirectUrl, UriKind.Absolute, out absoluteUrl))
            {
                logger.Error("Index => Error: Invalid redirect URL.");
                return View("Error");
            }

            logger.Info("Index => Redirecting to {0}", absoluteUrl);
            return Redirect(absoluteUrl.ToString());
        }
    }
}