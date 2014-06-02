using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FinApps.SSO.MVC5.Models;
using FinApps.SSO.RestClient_Base.Annotations;
using FinApps.SSO.RestClient_Base.Model;
using FinApps.SSO.RestClient_NET451;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using NLog;
using Quintsys.EnviromentConfigurationManager;

namespace FinApps.SSO.MVC5.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        #region private members and constructors

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly UserManager<ApplicationUser> _userManager;
        private IEnviromentConfigManager _configuration;
        private IFinAppsRestClient _client;

        [UsedImplicitly]
        public AccountController()
            : this(
                new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())), null, null
                )
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager, IEnviromentConfigManager config,
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

        private void LogModelStateErrors()
        {
            var errorMessage = new StringBuilder();
            foreach (ModelError error in ModelState.Values.SelectMany(modelState => modelState.Errors))
            {
                errorMessage.Append(error.ErrorMessage);
            }
            logger.Info("LogModelStateErrors => Error: Invalid ModelState. {0}", errorMessage.ToString());
        }

        #endregion

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                ModelState.AddModelError("", "Invalid username or password.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                LogModelStateErrors();
                return View(model);
            }

            if (_client == null)
                _client = InitializeApiClient();
            FinAppsUser user = await _client.NewUser(model.ToFinAppsUser());
            if (user.Errors != null && user.Errors.Any())
            {
                foreach (var error in user.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(model);
            }

            string userToken = user.UserToken;
            if (string.IsNullOrWhiteSpace(userToken))
            {
                logger.Warn("Register => Error: Invalid UserToken result.");
                ModelState.AddModelError("", "Unexpected error. Please try again.");
                return View(model);
            }
            logger.Info("Register => UserToken[{0}]", userToken);

            ApplicationUser applicationUser = model.ToApplicationUser(finAppsUserToken: userToken);
            IdentityResult identityResult = await _userManager.CreateAsync(applicationUser, model.Password);
            if (identityResult.Succeeded)
            {
                await SignInAsync(applicationUser, isPersistent: false);
                logger.Info("Register => Success. Redirecting to {0}", Url.Action("Index", "Home"));
                return RedirectToAction("Index", "Home");
            }
            AddErrors(identityResult);

            // If we got this far, something failed, redisplay form
            LogModelStateErrors();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            IdentityResult result =
                await
                    _userManager.RemoveLoginAsync(User.Identity.GetUserId(),
                        new UserLoginInfo(loginProvider, providerKey));
            ManageMessageId? message = result.Succeeded
                ? ManageMessageId.RemoveLoginSuccess
                : ManageMessageId.Error;
            return RedirectToAction("Manage", new {Message = message});
        }

        [ChildActionOnly]
        public PartialViewResult UpdateProfile()
        {
            ApplicationUser user = _userManager.FindById(User.Identity.GetUserId());
            var model = new UpdateProfileViewModel(user);

            return PartialView("_UpdateProfile", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateProfile(UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.StatusMessage = ManageMessageId.Error;
                return View("UpdateProfile", model);
            }    
            
            ApplicationUser user = _userManager.FindById(User.Identity.GetUserId());
            FinAppsCredentials credentials = user.ToFinAppsCredentials();

            FinAppsUser finAppsUser = model.ToFinAppsUser();
            
            if (_client == null)
                _client = InitializeApiClient();

            // updating profile on remote service
            FinAppsUser updatedUser = await _client.UpdateUserProfile(credentials, finAppsUser);
            if (updatedUser.Errors != null && updatedUser.Errors.Any())
            {
                foreach (var error in updatedUser.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View("UpdateProfile", model);
            }

            string userToken = updatedUser.UserToken;
            if (string.IsNullOrWhiteSpace(userToken))
            {
                logger.Warn("Update Profile => Error: Invalid UserToken result.");
                ModelState.AddModelError("", "Unexpected error. Please try again.");
                return View(model);
            }
            logger.Info("Update Profile => UserToken[{0}]", userToken);

            
            // updating local profile
            user.FinAppsUserToken = userToken;
            user.UpdateFromViewModel(model);
            IdentityResult identityResult = await _userManager.UpdateAsync(user);
            if (identityResult.Succeeded)
            {
                logger.Info("Profile Updated");

                await SignInAsync(user, isPersistent: false);

                return RedirectToAction("Manage", new { Message = ManageMessageId.ProfileUpdatedSuccess });
            }

            AddErrors(identityResult);
            return View("UpdateProfile", model);
        }

        [ChildActionOnly]
        public PartialViewResult Delete()
        {
            return PartialView("_Delete");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string userName)
        {
            if (!ModelState.IsValid)
            {
                return View("Delete");
            }

            ApplicationUser user = _userManager.FindById(User.Identity.GetUserId());
            if (user.UserName != userName)
            {
                logger.Warn("Forbidden user deletion attempted.");

                AuthenticationManager.SignOut();
                return new HttpUnauthorizedResult();
            }

            if (!string.IsNullOrWhiteSpace(user.FinAppsUserToken))
            {
                FinAppsCredentials credentials = user.ToFinAppsCredentials();

                if (_client == null)
                    _client = InitializeApiClient();

                // delete account on remote service
                FinAppsUser deletedUser = await _client.DeleteUser(credentials);
                if (deletedUser.Errors != null && deletedUser.Errors.Any())
                {
                    foreach (var error in deletedUser.Errors)
                    {
                        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }
                    LogModelStateErrors();
                    return View("Delete");
                }

                logger.Info("Account deleted from remote service.");
            }

            // delete local account
            IdentityResult identityResult = await _userManager.DeleteAsync(user);
            if (!identityResult.Succeeded) 
                return View("Delete");

            logger.Info("Local account deleted.");

            AuthenticationManager.SignOut();
            return RedirectToAction("Deleted", "Account");
        }

        [AllowAnonymous]
        public ActionResult Deleted()
        {
            return View();
        }
        
        public ActionResult Manage(ManageMessageId? message)
        {
            switch (message)
            {
                case ManageMessageId.ProfileUpdatedSuccess:
                    ViewBag.StatusMessage = "Your profile has been updated";
                    break;
                case ManageMessageId.ChangePasswordSuccess:
                    ViewBag.StatusMessage = "Your password has been changed.";
                    break;
                case ManageMessageId.SetPasswordSuccess:
                    ViewBag.StatusMessage = "Your password has been set.";
                    break;
                case ManageMessageId.RemoveLoginSuccess:
                    ViewBag.StatusMessage = "The external login was removed.";
                    break;
                case ManageMessageId.Error:
                    ModelState.AddModelError("", "An error has occurred.");
                    break;
                default:
                    ViewBag.StatusMessage = string.Empty;
                    break;
            }
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (!ModelState.IsValid) 
                    return View(model);
                
                ApplicationUser user = _userManager.FindById(User.Identity.GetUserId());
                FinAppsCredentials credentials = user.ToFinAppsCredentials();
                
                if (_client == null)
                    _client = InitializeApiClient();

                // updating password on remote service
                FinAppsUser updatedUser = await _client.UpdateUserPassword(credentials, model.OldPassword, model.NewPassword);
                if (updatedUser.Errors != null)
                {
                    foreach (var error in updatedUser.Errors)
                    {
                        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }
                    logger.Error("Manage => Error: Model validation errors.");
                    return View(model);
                }


                string userToken = updatedUser.UserToken;
                if (string.IsNullOrWhiteSpace(userToken))
                {
                    logger.Warn("Manage => Error: Invalid UserToken result.");
                    ModelState.AddModelError("", "Unexpected error. Please try again.");
                    return View(model);
                }
                logger.Info("Manage => UserToken[{0}]", userToken);


                // updating usertoken on local profile
                user.FinAppsUserToken = userToken;
                IdentityResult identityResult = await _userManager.UpdateAsync(user);
                if (!identityResult.Succeeded)
                {
                    ModelState.AddModelError("", "Unexpected error. Please try again.");
                    return View(model);
                }
                logger.Info("Manage => Profile Updated : UserToken");
                
                // update local password
                IdentityResult result = await _userManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    logger.Info("Password Updated.");
                    return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                AddErrors(result);
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (!ModelState.IsValid) 
                    return View(model);
                IdentityResult result = await _userManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Manage", new {Message = ManageMessageId.SetPasswordSuccess});
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider,
                Url.Action("ExternalLoginCallback", "Account", new {ReturnUrl = returnUrl}));
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await _userManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            // If the user does not have an account, then prompt the user to create an account
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
            return View("ExternalLoginConfirmation",
                new ExternalLoginConfirmationViewModel {UserName = loginInfo.DefaultUserName});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new {Message = ManageMessageId.Error});
            }
            var result = await _userManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new {Message = ManageMessageId.Error});
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model,
            string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser {UserName = model.UserName};
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = _userManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties {IsPersistent = isPersistent}, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = _userManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ProfileUpdatedSuccess,
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri, string userId = null)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            private string LoginProvider { get; set; }
            private string RedirectUri { get; set; }
            private string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties {RedirectUri = RedirectUri};
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion
    }
}