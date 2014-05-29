using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using FinApps.SSO.MVC4.Filters;
using FinApps.SSO.MVC4.Models;
using FinApps.SSO.RestClient_Base.Annotations;
using FinApps.SSO.RestClient_Base.Enums;
using FinApps.SSO.RestClient_Base.Model;
using FinApps.SSO.RestClient_NET40;
using Microsoft.Web.WebPages.OAuth;
using NLog;
using Quintsys.EnviromentConfigurationManager;
using WebMatrix.WebData;

namespace FinApps.SSO.MVC4.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        #region private members and constructor

        private readonly IFinAppsRestClient _client;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        public AccountController(IFinAppsRestClient client)
        {
            _client = client;
        }

        [UsedImplicitly]
        public AccountController()
        {
            IEnviromentConfigManager configuration = new EnviromentConfigManager();
            _client = new FinAppsRestClient(
                baseUrl: configuration.Get("FinAppsDemoUrl"),
                companyIdentifier: configuration.Get("FinAppsCompanyIdentifier"),
                companyToken: configuration.Get("FinAppsCompanyToken"));
        }

        private void ValidateServiceResult(ServiceResult serviceResult)
        {
            var successCodes = new[]
            {
                ResultCodeTypes.SUCCESSFUL, 
                ResultCodeTypes.ACCOUNT_AccountSaveSuccess,
                ResultCodeTypes.ACCOUNT_NewCustomerUserSavedSuccess
            };

            if (serviceResult != null && successCodes.Contains(serviceResult.Result))
                return;

            var errorMessage = serviceResult != null
                ? serviceResult.ResultString // => extended error information available on serviceResult.ResultObject
                : "Unexpected error. Please try again.";

            ModelState.AddModelError("", errorMessage);
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
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.Email, model.Password, persistCookie: model.RememberMe))
            {
                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                LogModelStateErrors();
                return View(model);
            }

            ServiceResult serviceResult = _client.NewUser(model.ToFinAppsUser());
            ValidateServiceResult(serviceResult);
            if (!ModelState.IsValid)
            {
                LogModelStateErrors();
                return View(model);
            }

            string userToken = serviceResult.GetUserToken();
            if (string.IsNullOrWhiteSpace(userToken))
            {
                logger.Warn("Register => Error: Invalid UserToken result.");
                ModelState.AddModelError("", "Unexpected error. Please try again.");
                return View(model);
            }

            logger.Info("Register => UserToken[{0}]", userToken);

            try
            {
                var propertyValues = new
                {
                    model.FirstName,
                    model.LastName,
                    model.PostalCode
                };
                WebSecurity.CreateUserAndAccount(model.Email, model.Password, propertyValues);
                WebSecurity.Login(model.Email, model.Password);

                logger.Info("Register => Success. Redirecting to {0}", Url.Action("Index", "Home"));
                return RedirectToAction("Index", "Home");
            }
            catch (MembershipCreateUserException e)
            {
                ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
            }

            // If we got this far, something failed, redisplay form
            LogModelStateErrors();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (
                    var scope = new TransactionScope(TransactionScopeOption.Required,
                        new TransactionOptions {IsolationLevel = IsolationLevel.Serializable}))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new {Message = message});
        }

        public ActionResult Manage(ManageMessageId? message)
        {
            switch (message)
            {
                case ManageMessageId.ChangePasswordSuccess:
                    ViewBag.StatusMessage = "Your password has been changed.";
                    break;
                case ManageMessageId.SetPasswordSuccess:
                    ViewBag.StatusMessage = "Your password has been set.";
                    break;
                case ManageMessageId.RemoveLoginSuccess:
                    ViewBag.StatusMessage = "The external login was removed.";
                    break;
                default:
                    ViewBag.StatusMessage = string.Empty;
                    break;
            }
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (!ModelState.IsValid)
                    return View(model);

                // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword,
                        model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                    return RedirectToAction("Manage", new {Message = ManageMessageId.ChangePasswordSuccess});

                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                    state.Errors.Clear();

                if (!ModelState.IsValid)
                    return View(model);

                try
                {
                    WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                    return RedirectToAction("Manage", new {Message = ManageMessageId.SetPasswordSuccess});
                }
                catch (Exception)
                {
                    ModelState.AddModelError("",
                        String.Format(
                            "Unable to create local account. An account with the name \"{0}\" may already exist.",
                            User.Identity.Name));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new {ReturnUrl = returnUrl}));
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result =
                OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new {ReturnUrl = returnUrl}));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }

            // User is new, ask for their desired membership name
            string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View("ExternalLoginConfirmation",
                new RegisterExternalLoginModel {Email = result.UserName, ExternalLoginData = loginData});
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider;
            string providerUserId;

            if (User.Identity.IsAuthenticated ||
                !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (var db = new UsersContext())
                {
                    UserProfile user =
                        db.UserProfiles.FirstOrDefault(
                            u => String.Equals(u.Email, model.Email, StringComparison.CurrentCultureIgnoreCase));
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile {Email = model.Email});
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.Email);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }

                    ModelState.AddModelError("Email", "User name already exists. Please enter a different user name.");
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            var externalLogins = (from account in accounts
                let clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider)
                select new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                })
                .ToList();

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 ||
                                       OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Helpers

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        private class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            private string Provider { get; set; }
            private string ReturnUrl { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return
                        "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return
                        "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return
                        "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return
                        "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        #endregion
    }
}