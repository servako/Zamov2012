using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using Zamov.Models;
using System.Text.RegularExpressions;
using Zamov.Helpers;
using System.Net.Mail;
using Zamov.srSms;

namespace Zamov.Controllers
{

    [HandleError]
    public class AccountController : Controller
    {

        // This constructor is used by the MVC framework to instantiate the controller using
        // the default forms authentication and membership providers.

        public AccountController()
            : this(null, null)
        {
        }

        // This constructor is not used by the MVC framework but is instead provided for ease
        // of unit testing this type. See the comments at the end of this file for more
        // information.
        public AccountController(IFormsAuthentication formsAuth, IMembershipService service)
        {
            FormsAuth = formsAuth ?? new FormsAuthenticationService();
            MembershipService = service ?? new AccountMembershipService();
        }

        public IFormsAuthentication FormsAuth
        {
            get;
            private set;
        }

        public IMembershipService MembershipService
        {
            get;
            private set;
        }

        [BreadCrumb(ResourceName = "LogOn", Url = "/LogOn")]
        public ActionResult LogOn()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings",
            Justification = "Needs to take same parameter type as Controller.Redirect()")]
        public ActionResult LogOn(string userName, string password, bool rememberMe, string returnUrl)
        {
            if (!ValidateLogOn(userName, password))
            {
                return View();
            }

            FormsAuth.SignIn(userName, rememberMe);

            ProfileCommon profile = ProfileCommon.Create(userName);
            if (profile.DealerEmployee)
            {
                return RedirectToAction("Orders",  "DealerCabinet");
            }
            if (!String.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else 
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult LogOff()
        {
            SystemSettings.CurrentDealer = null;
            FormsAuth.SignOut();
            SystemSettings.EmptyCart();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ResetPassword()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [CaptchaValidation("captcha")]
        public ActionResult ResetPassword(string email, bool captchaValid)
        {
            MembershipUser user = ValidateResetPassword(email, captchaValid);
            if (user != null)
            {
                string newPassword = user.ResetPassword();
                
                MailHelper.SendTemplate("no-reply@zamov.ua", 
                    new List<MailAddress> { new MailAddress(email) }, 
                    "Zamov.ua",
                    "resetPassword", 
                    SystemSettings.CurrentLanguage, 
                    false, 
                    newPassword);
                
                return RedirectToAction("PasswordReseted");
            }
            return View();
        }

        public ActionResult PasswordReseted()
        {
            return View("PasswordReseted" + SystemSettings.CurrentLanguage);
        }

        private MembershipUser ValidateResetPassword(string email, bool captchaValid)
        {
            MembershipUser user = null;
            if (string.IsNullOrEmpty(email))
                ModelState.AddModelError("email", ResourcesHelper.GetResourceString("EmailRequired"));
            Regex regex = new Regex("^(?:[a-zA-Z0-9_'^&amp;/+-])+(?:\\.(?:[a-zA-Z0-9_'^&amp;/+-])+)*@(?:(?:\\[?(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?))\\.){3}(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\]?)|(?:[a-zA-Z0-9-]+\\.)+(?:[a-zA-Z]){2,}\\.?)$");
            if (!regex.IsMatch(email))
                ModelState.AddModelError("email", ResourcesHelper.GetResourceString("EmailIncorrect"));
            if (!captchaValid)
                ModelState.AddModelError("captchaCheck", ResourcesHelper.GetResourceString("IncorrectCaptcha"));
            if (ModelState.IsValid)
            {
                user = Membership.GetUser(email);
                if (user == null || !user.IsApproved)
                {
                    ModelState.AddModelError("_FORM", ResourcesHelper.GetResourceString("NoUserMatchesEmail"));
                    user = null;
                }
            }
            return user;
        }

        [BreadCrumb(ResourceName = "Register", Url = "/LogOn")]
        public ActionResult Register()
        {

            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            if (SystemSettings.MemberProperties != null)
            {
                ViewData["email"] = SystemSettings.MemberProperties.Email;
                ViewData["firstName"] = SystemSettings.MemberProperties.FirstName;
                ViewData["lastName"] = SystemSettings.MemberProperties.LastName;
                ViewData["deliveryAddress"] = SystemSettings.MemberProperties.DeliveryAddress;
                ViewData["mobilePhone"] = SystemSettings.MemberProperties.MobilePhone;
                ViewData["phone"] = SystemSettings.MemberProperties.Phone;
                ViewData["city"] = SystemSettings.MemberProperties.City;
            }
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [CaptchaValidation("captcha")]
        public ActionResult Register(
            string userName,
            string email,
            string password,
            string confirmPassword,
            string firstName,
            string lastName,
            string deliveryAddress,
            string mobilePhone,
            string phone,
            string city,
            bool captchaValid
            )
        {

            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            Regex r = new Regex(@"\(|\)|\-| ", RegexOptions.IgnoreCase);
            mobilePhone = r.Replace(mobilePhone, "");
            phone = r.Replace(phone, "");

            if (ValidateRegistration(email, password, confirmPassword, firstName, lastName, mobilePhone, phone, captchaValid))
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(email, password, email);
                if (createStatus == MembershipCreateStatus.Success)
                {
                    MembershipUser user = Membership.GetUser(email);
                    user.IsApproved = false;
                    Membership.UpdateUser(user);
                    ProfileCommon profile = ProfileCommon.Create(email) as ProfileCommon;
                    profile.FirstName = firstName;
                    profile.LastName = lastName;
                    profile.Phone = string.Format("38{0}", phone);
                    profile.MobilePhone = string.Format("38{0}",mobilePhone);
                    profile.DeliveryAddress = deliveryAddress;
                    profile.City = city;
                    profile.Save();
                    Roles.AddUserToRole(email, "Customers");
                    if (SystemSettings.Cart.Id > 0)
                    {
                        Cart cart = SystemSettings.Cart;
                        using (OrderStorage context = new OrderStorage())
                        {
                            Cart loadedCart = context.Carts.Where(c => c.Id == cart.Id).Select(c => c).First();
                            loadedCart.Orders.Load();
                            Guid userId = (Guid)Membership.GetUser(email).ProviderUserKey;
                            context.AcceptAllChanges();
                            foreach (Order item in loadedCart.Orders)
                                item.UserId = userId;
                            context.SaveChanges();
                        }
                        SystemSettings.EmptyCart();
                    }
                    string linkBase = "http://test.zamov.com";
                    if (Request.Url.AbsoluteUri.StartsWith("http://zamov.net") || Request.Url.AbsoluteUri.StartsWith("http://www.zamov.net"))
                        linkBase = "http://zamov.net";
                    else if (Request.Url.AbsoluteUri.StartsWith("http://zamov.com") || Request.Url.AbsoluteUri.StartsWith("http://www.zamov.com"))
                        linkBase = "http://zamov.com";
                    else if (Request.Url.AbsoluteUri.StartsWith("http://zamov.ua") || Request.Url.AbsoluteUri.StartsWith("http://www.zamov.ua"))
                        linkBase = "http://zamov.ua";

                    bool sendEmail = MailHelper.SendTemplate("no-reply@zamov.ua",
                        new List<MailAddress> { new MailAddress(email) },
                        "Zamov",
                        "activateAccount",
                        SystemSettings.CurrentLanguage,
                        false,
                        linkBase + "/Account/UserEmailVerified?guid=" + user.ProviderUserKey);

                    if (!sendEmail)
                    {
                        // костыль на случай проблем с отправкой почты для подтверждения регистрации, шлются смс для ручной активации пользователя
                        var contacts = new srSms.contact[2];
                        contacts[0] = new contact { phone = "380997389163" };
                        contacts[1] = new contact { phone = "380677796536" };
                        SmsHelper.SendTemplate("Zamov", contacts, "errorSendUserEmailForActivation", String.Empty, email);
                    }
                    
                    return RedirectToAction("UserEmailVerification", new { email = email });
                }
                else
                {
                    ModelState.AddModelError("_FORM", ErrorCodeToString(createStatus));
                }
            }
            // If we got this far, something failed, redisplay form
            return View();
        }

        public ActionResult UserEmailVerification(string email)
        {
            ViewData["email"] = email;
            return View("UserEmailVerification" + SystemSettings.CurrentLanguage);
        }

        public ActionResult UserEmailVerified(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return RedirectToAction("Index", "Home");

            MembershipUser user = Membership.GetUser(new Guid(guid));
            
            if(user == null)
                return RedirectToAction("Index", "Home");

            user.IsApproved = true;
            Membership.UpdateUser(user);
            
            FormsAuth.SignIn(user.UserName, true);

            return View("UserEmailVerified" + SystemSettings.CurrentLanguage);
        }

        [Authorize]
        [BreadCrumb(ResourceName = "ChangePassword", Url = "/ChangePassword")]
        public ActionResult ChangePassword()
        {

            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            return View();
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Exceptions result in password not being changed.")]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {

            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            if (!ValidateChangePassword(currentPassword, newPassword, confirmPassword))
            {
                return View();
            }

            try
            {
                if (MembershipService.ChangePassword(User.Identity.Name, currentPassword, newPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("_FORM", Controllers.ResourcesHelper.GetResourceString("TheCurrentPasswordIsIncorrectOrTheNewPasswordIsInvalid"));
                    return View();
                }
            }
            catch
            {
                ModelState.AddModelError("_FORM", Controllers.ResourcesHelper.GetResourceString("TheCurrentPasswordIsIncorrectOrTheNewPasswordIsInvalid"));
                return View();
            }
        }

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity is WindowsIdentity)
            {
                throw new InvalidOperationException("Windows authentication is not supported.");
            }
        }

        #region Validation Methods

        private bool ValidateChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (String.IsNullOrEmpty(currentPassword))
            {
                ModelState.AddModelError("currentPassword", "You must specify a current password.");
            }
            if (newPassword == null || newPassword.Length < MembershipService.MinPasswordLength)
            {
                ModelState.AddModelError("newPassword",
                    String.Format(CultureInfo.CurrentCulture,
                         "You must specify a new password of {0} or more characters.",
                         MembershipService.MinPasswordLength));
            }

            if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
            }

            return ModelState.IsValid;
        }

        private bool ValidateLogOn(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("username", ResourcesHelper.GetResourceString("UserNameRequired"));
            }
            if (String.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", ResourcesHelper.GetResourceString("PasswordRequired"));
            }
            if (!MembershipService.ValidateUser(userName, password))
            {
                ModelState.AddModelError("_FORM", ResourcesHelper.GetResourceString("IncorrectUserNameOrPassword"));
            }

            return ModelState.IsValid;
        }

        private bool ValidateRegistration(string email, string password, string confirmPassword, string firstName, string lastName, string mobilePhone, string phone, bool captchaValid)
        {
            if (!captchaValid)
            {
               // ModelState.AddModelError("captchaCheck", ResourcesHelper.GetResourceString("IncorrectCaptcha"));
            }
            if (String.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("email", ResourcesHelper.GetResourceString("EmailRequired"));
            }
            else
            {
                Regex regex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
                if (!regex.IsMatch(email))
                    ModelState.AddModelError("email", ResourcesHelper.GetResourceString("EmailIncorrect"));
            }

            if (password == null || password.Length < MembershipService.MinPasswordLength)
            {
                ModelState.AddModelError("password",
                    String.Format(CultureInfo.CurrentCulture,
                         ResourcesHelper.GetResourceString("PasswordShouldContain") + " {0}",
                         MembershipService.MinPasswordLength));
            }
            if (!String.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", ResourcesHelper.GetResourceString("PasswordAndConfirmationShouldMatch"));
            }
            if (string.IsNullOrEmpty(firstName))
                ModelState.AddModelError("firstName", ResourcesHelper.GetResourceString("FirstNameRequired"));
            if (string.IsNullOrEmpty(lastName))
                ModelState.AddModelError("lastName", ResourcesHelper.GetResourceString("LastNameRequired"));
            if (string.IsNullOrEmpty(mobilePhone))
                ModelState.AddModelError("mobilePhone", ResourcesHelper.GetResourceString("PhoneRequired"));
            Regex phoneCheck = new Regex(@"^\d{3}[0-9]{7}$");
            if(!string.IsNullOrEmpty(mobilePhone) && !phoneCheck.IsMatch(mobilePhone))
                ModelState.AddModelError("mobilePhone", ResourcesHelper.GetResourceString("PhoneIncorrect"));
            if (!string.IsNullOrEmpty(phone) && !phoneCheck.IsMatch(phone))
                ModelState.AddModelError("phone", ResourcesHelper.GetResourceString("PhoneIncorrect"));
            return ModelState.IsValid;
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return ResourcesHelper.GetResourceString("ErrDuplicateUserName");

                case MembershipCreateStatus.DuplicateEmail:
                    return ResourcesHelper.GetResourceString("ErrDuplicateEmail");

                case MembershipCreateStatus.InvalidPassword:
                    return ResourcesHelper.GetResourceString("ErrInvalidPassword");

                case MembershipCreateStatus.InvalidEmail:
                    return ResourcesHelper.GetResourceString("ErrInvalidEmail");

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return ResourcesHelper.GetResourceString("ErrInvalidUserName");

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }

    // The FormsAuthentication type is sealed and contains static members, so it is difficult to
    // unit test code that calls its members. The interface and helper class below demonstrate
    // how to create an abstract wrapper around such a type in order to make the AccountController
    // code unit testable.

    public interface IFormsAuthentication
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthentication
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }
        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        MembershipCreateStatus CreateUser(string userName, string password, string email);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
    }

    public class AccountMembershipService : IMembershipService
    {
        private MembershipProvider _provider;

        public AccountMembershipService()
            : this(null)
        {
        }

        public AccountMembershipService(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public bool ValidateUser(string userName, string password)
        {
            return _provider.ValidateUser(userName, password);
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            MembershipCreateStatus status;
            _provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
            return currentUser.ChangePassword(oldPassword, newPassword);
        }
    }
}
