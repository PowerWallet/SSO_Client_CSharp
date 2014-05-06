using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using FinApps.SSO.MVC5.Models;
using FinApps.SSO.MVC5.Services;
using FinApps.SSO.RestClient;
using FinApps.SSO.RestClient.Annotations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FinApps.SSO.MVC5.Controllers
{
    [Authorize]
    public class PfmController : Controller
    {
        #region private members and constructors

        private readonly UserManager<ApplicationUser> _userManager;
        private IConfig _configuration;
        private IFinAppsRestClient _client;

        [UsedImplicitly]
        public PfmController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())), null, null)
        {
        }

        public PfmController(UserManager<ApplicationUser> userManager, IConfig config,
            IFinAppsRestClient finAppsRestClient)
        {
            _userManager = userManager;
            _configuration = config;
            _client = finAppsRestClient;
        }

        private FinAppsRestClient InitializeApiClient()
        {
            if (_configuration == null)
                _configuration = new Config();

            return new FinAppsRestClient(
                baseUrl: _configuration.Get("FinAppsDemoUrl"),
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
                return View("Error");

            if (_client == null)
                _client = InitializeApiClient();
            
            string redirectUrl = await _client.NewSession(user.ToFinAppsCredentials(), Request.UserHostAddress);
            if (string.IsNullOrEmpty(redirectUrl))
                return View("Error");
            
            Uri absoluteUrl;
            return Uri.TryCreate(redirectUrl, UriKind.Absolute, out absoluteUrl)
                ? Redirect(absoluteUrl.ToString())
                : (ActionResult) View("Error");
        }
    }
}