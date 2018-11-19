using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using NLog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ToDoApplication.Code;
using ToDoApplication.Code.Helper;
using ToDoApplication.Models;
using ToDoApplication.Models.Identity;
using ToDoApplication.Providers;
using ToDoApplication.Results;
using TodoData.Models.User;
using ToDoData.Enum;

namespace ToDoApplication.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private UserManager _userManager;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public AccountController()
        {
        }

        public AccountController(UserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public UserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<UserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public SignInManager SignInManager
        {
            get { return Request.GetOwinContext().Get<SignInManager>(); }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            //Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            SignInManager.SignOut();
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        //[Route("ManageInfo")]
        //public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        //{
        //    var user = await UserManager.FindByIdAsync(Convert.ToInt64(User.Identity.GetUserId()));

        //    if (user == null)
        //    {
        //        return null;
        //    }

        //    List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

        //    foreach (IdentityUserLogin linkedAccount in user.Logins)
        //    {
        //        logins.Add(new UserLoginInfoViewModel
        //        {
        //            LoginProvider = linkedAccount.LoginProvider,
        //            ProviderKey = linkedAccount.ProviderKey
        //        });
        //    }

        //    if (user.PasswordHash != null)
        //    {
        //        logins.Add(new UserLoginInfoViewModel
        //        {
        //            LoginProvider = LocalLoginProvider,
        //            ProviderKey = user.UserName,
        //        });
        //    }

        //    return new ManageInfoViewModel
        //    {
        //        LocalLoginProvider = LocalLoginProvider,
        //        Email = user.UserName,
        //        Logins = logins,
        //        ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
        //    };
        //}

        // POST api/Account/ChangePassword

        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(Convert.ToInt64(User.Identity.GetUserId()), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        [AllowAnonymous]
        [Route("ResetPassword")]
        public async Task<IHttpActionResult> ResetPassword(ResetPasswordBindingModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var user = UserManager2.FindUserInfo(model.Email);
                if (user == null) return BadRequest("email not found");

                var newPassword = NameGenerator.RandomString(7);
                var emailResult = EmailManager.SendNewPassword(model.Email, newPassword);

                if (emailResult)
                {
                    //update password in db
                    IdentityResult result = await UserManager.AddPasswordAsync(user.Id, newPassword);

                    if (!result.Succeeded)
                    {
                        return GetErrorResult(result);
                    }

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"ResetPassword error: {ex}");
                return BadRequest(ex.Message);
            }

            return BadRequest();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(Convert.ToInt64(User.Identity.GetUserId()), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(Convert.ToInt64(User.Identity.GetUserId()),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(Convert.ToInt64(User.Identity.GetUserId()));
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(Convert.ToInt64(User.Identity.GetUserId()),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            var user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                SignInManager.SignOut();
                //Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await UserManager.CreateIdentityAsync(user, OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookiesIdentity = await UserManager.CreateIdentityAsync(user, CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                SignInManager.SignIn(user, false, false);
                //Authentication.SignIn(properties, oAuthIdentity, cookiesIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);

                var userInfo = UserManager2.CreateUserInfo("", externalLogin.UserName, "", "", "");

                user = new User()
                {
                    UserName = externalLogin.UserName,
                    PasswordHash = null,
                    IsActive = true,
                    UserType = (int)UserTypeEnum.Client,
                    UserInfoId = userInfo.Id,
                    AccountPlanId = (int)AccountPlanEnum.Start,
                    Registration = DateTime.Now,
                    LastUpdate = DateTime.Now
                };

                IdentityResult result = await UserManager.CreateAsync(user);

                IdentityResult loginResult = await UserManager.AddLoginAsync(user.Id, new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey));

                SignInManager.SignIn(user, false, false);
                //Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ProcessExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ProcessExternalLogin", Name = "ProcessExternalLogin")]
        public async Task<IHttpActionResult> ProcessExternalLogin()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            var user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (!hasRegistered)
            {
                //зарегестрировать и авторизировать пользователя
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);

                var userInfo = UserManager2.CreateUserInfo("", externalLogin.UserName, "", "", "");

                user = new User()
                {
                    UserName = externalLogin.UserName,
                    PasswordHash = null,
                    IsActive = true,
                    UserType = (int)UserTypeEnum.Client,
                    UserInfoId = userInfo.Id,
                    AccountPlanId = (int)AccountPlanEnum.Start,
                    Registration = DateTime.Now,
                    LastUpdate = DateTime.Now
                };

                IdentityResult result = await UserManager.CreateAsync(user);

                IdentityResult loginResult = await UserManager.AddLoginAsync(user.Id, new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey));

                //SignInManager.SignIn(user, false, false);
                //Authentication.SignIn(identity);
            }

            //авторизировать пользователя
            SignInManager.SignOut();
            //Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            ClaimsIdentity oAuthIdentity = await UserManager.CreateIdentityAsync(user, OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await UserManager.CreateIdentityAsync(user, CookieAuthenticationDefaults.AuthenticationType);

            AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
            SignInManager.SignIn(user, false, false);
            //Authentication.SignIn(properties, oAuthIdentity, cookiesIdentity);

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Login
        [HttpPost]
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<IHttpActionResult> Login(LoginBindingModel model)
        {
            if (ModelState.IsValid)
            {
                var result = SignInManager.PasswordSignIn(model.UserName, model.Password, false, false);
                if (result == SignInStatus.Success)
                {
                    var user = UserManager2.FindUser(model.UserName, model.Password);
                    ClaimsIdentity oAuthIdentity = await UserManager.CreateIdentityAsync(user, OAuthDefaults.AuthenticationType);
                    ClaimsIdentity cookiesIdentity = await UserManager.CreateIdentityAsync(user, CookieAuthenticationDefaults.AuthenticationType);
                    AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                    Authentication.SignIn(properties, oAuthIdentity, cookiesIdentity);
                    return Ok();
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            return BadRequest(ModelState);
        }

        // POST api/Account/Register
        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            logger.Log(LogLevel.Info, $"Register({model.Email})");
            if (!ModelState.IsValid)
            {
                logger.Log(LogLevel.Error, $"Register({model.Email}). Error: model state is not invalid");
                return BadRequest(ModelState);
            }

            //var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
            var userInfo = UserManager2.CreateUserInfo("", "", "", "", "");

            var user = new User()
            {
                UserName = model.Email,
                PasswordHash = model.Password,
                IsActive = true,
                UserInfoId = userInfo.Id,
                UserType = (int)UserTypeEnum.Client,
                AccountPlanId = (int)AccountPlanEnum.Start,
                Registration = DateTime.Now,
                LastUpdate = DateTime.Now
            };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            //SignInManager.SignIn(user, false, false);
            ClaimsIdentity oAuthIdentity = await UserManager.CreateIdentityAsync(user, OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await UserManager.CreateIdentityAsync(user, CookieAuthenticationDefaults.AuthenticationType);
            AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
            Authentication.SignIn(properties, oAuthIdentity, cookiesIdentity);

            return Ok();
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            //var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
            var user = new User()
            {
                UserName = model.Email,
                PasswordHash = null,//???
                IsActive = true,
                UserType = (int)UserTypeEnum.Client,
                AccountPlanId = (int)AccountPlanEnum.Start,
                Registration = DateTime.Now,
                LastUpdate = DateTime.Now
            };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
