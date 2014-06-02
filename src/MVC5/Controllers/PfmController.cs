using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using FinApps.SSO.MVC5.Models;
using FinApps.SSO.RestClient_Base.Annotations;
using FinApps.SSO.RestClient_Base.Model;
using FinApps.SSO.RestClient_NET451;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NLog;
using Quintsys.EnviromentConfigurationManager;

namespace FinApps.SSO.MVC5.Controllers
{
    [Authorize]
    public class PfmController : Controller
    {
        #region private members and constructors

        private readonly UserManager<ApplicationUser> _userManager;
        private IEnviromentConfigManager _configuration;
        private IFinAppsRestClient _client;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        [UsedImplicitly]
        public PfmController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())), null, null)
        {
        }

        public PfmController(UserManager<ApplicationUser> userManager, IEnviromentConfigManager config,
            IFinAppsRestClient finAppsRestClient)
        {
            _userManager = userManager;
            _configuration = config;
            _client = finAppsRestClient;
        }

        private FinAppsRestClient InitializeApiClient()
        {
            if (_configuration == null)
                _configuration = new EnviromentConfigManager();

            return new FinAppsRestClient(
                baseUrl: _configuration.Get("FinAppsUrl"),
                companyIdentifier: _configuration.Get("FinAppsCompanyIdentifier"),
                companyToken: _configuration.Get("FinAppsCompanyToken"));
        }

        private static bool IsValidUser(ApplicationUser user)
        {
            return user != null && !string.IsNullOrWhiteSpace(user.FinAppsUserToken);
        }

        #endregion

        public async Task<ActionResult> Index()
        {
            ApplicationUser user = _userManager.FindById(User.Identity.GetUserId());
            if (!IsValidUser(user))
            {
                logger.Error("Index => Error: Not a valid user.");
                return View("Error");
            }

            if (_client == null)
                _client = InitializeApiClient();

            // creating new session on remote server
            FinAppsUser newSessionUser = await _client.NewSession(user.ToFinAppsCredentials(), Request.UserHostAddress);
            if (newSessionUser.Errors != null)
            {
                logger.Error("Index => Error: Invalid redirect URL.");
                return View("Error");
            }

            string redirectUrl = newSessionUser.SessionRedirectUrl;
            if (string.IsNullOrEmpty(redirectUrl))
            {
                logger.Error("Index => Error: Invalid redirect URL.");
                return View("Error");
            }
            
            Uri absoluteUrl;
            if (!Uri.TryCreate(redirectUrl, UriKind.Absolute, out absoluteUrl))
            {
                logger.Error("Index => Error: Invalid redirect URL.");
                return (ActionResult) View("Error");
            }
            
            logger.Info("Index => Redirecting to {0}", absoluteUrl);
            return Redirect(absoluteUrl.ToString());
        }
    }
}