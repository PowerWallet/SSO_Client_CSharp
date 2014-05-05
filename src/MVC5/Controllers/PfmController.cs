using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using FinApps.SSO.MVC5.Models;
using FinApps.SSO.MVC5.Services;
using FinApps.SSO.RestClient;
using FinApps.SSO.RestClient.Annotations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;

namespace FinApps.SSO.MVC5.Controllers
{
    [Authorize]
    public class PfmController : Controller
    {
        #region private members and constructors

        private readonly IFinAppsRestClient _client;
        private readonly UserManager<ApplicationUser> _userManager;

        [UsedImplicitly]
        public PfmController()
            : this(
                new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())),
                new Config(), null)
        {
        }

        public PfmController(UserManager<ApplicationUser> userManager, IConfig config,
            IFinAppsRestClient finAppsRestClient)
        {
            _userManager = userManager;

            IConfig configuration = config ?? new Config();

            if (finAppsRestClient != null)
            {
                _client = finAppsRestClient;
            }
            else
            {
                //string apiBaseUrl = configuration.Get("FinAppsLiveUrl");
                string baseUrl = configuration.Get("FinAppsDemoUrl");
                string companyIdentifier = configuration.Get("FinAppsCompanyIdentifier");
                string companytoken = configuration.Get("FinAppsCompanyToken");

                _client = new FinAppsRestClient(baseUrl, companyIdentifier, companytoken);
            }
        }

        private static bool IsValidUser(ApplicationUser user)
        {
            return user != null && !string.IsNullOrWhiteSpace(user.FinAppsUserToken);
        }

        private static bool IsValidServiceResult(ServiceResult serviceResult)
        {
            return serviceResult != null && serviceResult.Result == ResultCodeTypes.SUCCESSFUL;
        }

        #endregion

        public async Task<ActionResult> Index()
        {
            ApplicationUser user = _userManager.FindById(User.Identity.GetUserId());
            if (!IsValidUser(user))
                return View("Error");

            ServiceResult serviceResult = await _client.NewSession(user.ToFinAppsCredentials(), Request.UserHostAddress);
            if (!IsValidServiceResult(serviceResult))
                return View("Error");

            var ssoResponse = JsonConvert.DeserializeObject<NewSessionResponse>(serviceResult.ResultObject.ToString());
            if (ssoResponse == null)
                return View("Error");

            Uri absoluteUrl;
            if (Uri.TryCreate(ssoResponse.RedirectToUrl, UriKind.Absolute, out absoluteUrl))
                return Redirect(absoluteUrl.ToString());

            return View("Error");
        }
    }
}