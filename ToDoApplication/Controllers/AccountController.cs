using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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
using System.Linq;

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

        // POST api/Account/Login
        [HttpPost]
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<object> Login(LoginBindingModel model)
        {
            if (ModelState.IsValid)
            {
                var result = SignInManager.PasswordSignIn(model.email, model.password, false, false);
                if (result == SignInStatus.Success)
                {
                    var user = UserManager2.FindUser(model.email);
                    var usermodel = UserManager2.GetUserModel(user.Id);

                    ClaimsIdentity oAuthIdentity = await UserManager.CreateIdentityAsync(user, OAuthDefaults.AuthenticationType);
                    ClaimsIdentity cookiesIdentity = await UserManager.CreateIdentityAsync(user, CookieAuthenticationDefaults.AuthenticationType);
                    AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                    Authentication.SignIn(properties, oAuthIdentity, cookiesIdentity);
                    var token = GetToken(model.email, model.password);

                    return new
                    {
                        token = token,
                        user = usermodel
                    };
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
        public async Task<object> Register()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            byte[] buffer = null;
            string filename;
            string email = string.Empty;
            string username = string.Empty;
            string password = string.Empty;
            string confirmpassword = string.Empty;

            var fileManager = new AzureFileManager();
            foreach (var file in provider.Contents)
            {
                if (file.Headers.ContentDisposition.Name.Contains("email"))
                {
                    email = await file.ReadAsStringAsync();
                }
                else if (file.Headers.ContentDisposition.Name.Contains("username"))
                {
                    username = await file.ReadAsStringAsync();
                }
                else if (file.Headers.ContentDisposition.Name.Contains("password"))
                {
                    password = await file.ReadAsStringAsync();
                }
                else if (file.Headers.ContentDisposition.Name.Contains("confirmpassword"))
                {
                    confirmpassword = await file.ReadAsStringAsync();
                }
                else
                {
                    filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                    buffer = await file.ReadAsByteArrayAsync();
                }
            }

            var model = new RegisterBindingModel()
            {
                email = email,
                username = username,
                password = password,
                confirmpassword = confirmpassword,
            };

            logger.Log(LogLevel.Info, $"Register({model.email})");
            if (!ModelState.IsValid)
            {
                logger.Log(LogLevel.Error, $"Register({model.email}). Error: model state is not invalid");
                return BadRequest(ModelState);
            }

            //var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
            var userInfo = UserManager2.CreateUserInfo("", username, "", "", "");

            var user = new User()
            {
                UserName = model.email,
                PasswordHash = model.password,
                IsActive = true,
                UserInfoId = userInfo.Id,
                UserType = (int)UserTypeEnum.Client,
                AccountPlanId = (int)AccountPlanEnum.Start,
                Registration = DateTime.Now,
                LastUpdate = DateTime.Now
            };

            IdentityResult result = await UserManager.CreateAsync(user, model.password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            user = UserManager2.Create(user);
            //SignInManager.SignIn(user, false, false);
            ClaimsIdentity oAuthIdentity = await UserManager.CreateIdentityAsync(user, OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await UserManager.CreateIdentityAsync(user, CookieAuthenticationDefaults.AuthenticationType);
            AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
            Authentication.SignIn(properties, oAuthIdentity, cookiesIdentity);

            var token = GetToken(model.email, model.password);

            //avatar
            var uploadResult = fileManager.UploadFileAsync(buffer, $"{user.Id}.png");//pass file stream

            if (string.IsNullOrEmpty(uploadResult.Result)) return BadRequest(uploadResult.Result);


            var info = UserManager2.FindUserInfo(user.UserInfoId);
            info.PhotoUrl = uploadResult.Result;
            UserManager2.UpdateInfo(info);

            var usermodel = UserManager2.GetUserModel(user.UserName);

            return new
            {
                token = token,
                user = usermodel
            };
        }

        /*
         * [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<object> Register(RegisterBindingModel model)
        {
            logger.Log(LogLevel.Info, $"Register({model.email})");
            if (!ModelState.IsValid)
            {
                logger.Log(LogLevel.Error, $"Register({model.email}). Error: model state is not invalid");
                return BadRequest(ModelState);
            }

            //var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
            var userInfo = UserManager2.CreateUserInfo("", "", "", "", "");

            var user = new User()
            {
                UserName = model.email,
                PasswordHash = model.password,
                IsActive = true,
                UserInfoId = userInfo.Id,
                UserType = (int)UserTypeEnum.Client,
                AccountPlanId = (int)AccountPlanEnum.Start,
                Registration = DateTime.Now,
                LastUpdate = DateTime.Now
            };

            IdentityResult result = await UserManager.CreateAsync(user, model.password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            user = UserManager2.Create(user);
            //SignInManager.SignIn(user, false, false);
            ClaimsIdentity oAuthIdentity = await UserManager.CreateIdentityAsync(user, OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await UserManager.CreateIdentityAsync(user, CookieAuthenticationDefaults.AuthenticationType);
            AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
            Authentication.SignIn(properties, oAuthIdentity, cookiesIdentity);

            var token = GetToken(model.email, model.password);
            var usermodel = UserManager2.GetUserModel(user.UserName);

            return new
            {
                token = token,
                user = usermodel
            };
        }
         */

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

        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId<long>(), 
                model.oldpassword,
                model.newpassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
            user = UserManager2.Update(user);

            return Ok();
        }

        [AllowAnonymous]
        [Route("ResetPassword")]
        public async Task<IHttpActionResult> ResetPassword(ResetPasswordBindingModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var user = UserManager2.FindUser(model.email);
                if (user == null) return BadRequest("email not found");

                var newPassword = NameGenerator.RandomString(7);
                var emailResult = EmailManager.SendNewPassword(model.email, newPassword);

                if (emailResult)
                {
                    //IdentityResult result = await .SetPasswordHashAsync(user, newPassword);

                    var provider = new DpapiDataProtectionProvider("YourAppName");
                    UserManager.UserTokenProvider = new DataProtectorTokenProvider<User, long>(provider.Create("ASP.NET Identity")) 
                        as IUserTokenProvider<User, long>;

                    var resetToken = UserManager.GeneratePasswordResetToken(user.Id);
                    IdentityResult result = UserManager.ResetPassword(user.Id, resetToken, newPassword);

                    if (!result.Succeeded)
                    {
                        return GetErrorResult(result);
                    }

                    user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
                    user = UserManager2.Update(user);

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

            IdentityResult result = await UserManager.AddPasswordAsync(Convert.ToInt64(User.Identity.GetUserId()), model.password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        [Route("SetEmail")]
        public async Task<IHttpActionResult> SetNewEmail(SetEmailBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            /*
            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId<long>(),
                model.oldpassword,
                model.newpassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
            user = UserManager2.Update(user);
            */

            return Ok();
        }

        [Route("UpdateImage")]
        public async Task<System.Web.Http.IHttpActionResult> UpdateAvatar()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var user = UserManager2.FindUser(User.Identity.Name);
            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            var fileManager = new AzureFileManager();
            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                var buffer = await file.ReadAsByteArrayAsync();
                //Do whatever you want with filename and its binary data.

                var result = fileManager.UploadFileAsync(buffer, $"{user.Id}.png");//pass file stream

                if (string.IsNullOrEmpty(result.Result)) return BadRequest(result.Result);


                var info = UserManager2.FindUserInfo(user.UserInfoId);
                info.PhotoUrl = result.Result;
                UserManager2.UpdateInfo(info);
            }

            return Ok();
        }

        [Route("SetNotification")]
        public async Task<IHttpActionResult> SetNotification(SetPushBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = UserManager2.FindUser(User.Identity.Name);
            var userInfo = UserManager2.FindUserInfo(user.UserInfoId);
            userInfo.Notification = model.turnOn;
            UserManager2.UpdateInfo(userInfo);

            return Ok();
        }

        [Route("SetPushNotification")]
        public async Task<IHttpActionResult> SetPushNotification(SetPushBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = UserManager2.FindUser(User.Identity.Name);
            var userInfo = UserManager2.FindUserInfo(user.UserInfoId);
            userInfo.Push = model.turnOn;
            UserManager2.UpdateInfo(userInfo);

            return Ok();
        }

        [Route("FindUser")]
        public async Task<object> FindUser(FindUserBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = UserManager2.FindUser(User.Identity.Name);
            var info = UserManager2.FindUserInfo(user.UserInfoId);

            var result = new SimpleUserModel()
            {
                id = user.Id,
                email = user.UserName,
                username = info.Name,
                avatar = info.PhotoUrl
            };

            return result;
        }

        [Route("SetFavoriteNotification")]
        public async Task<IHttpActionResult> SetFavoriteNotification(SetPushBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = UserManager2.FindUser(User.Identity.Name);
            var userInfo = UserManager2.FindUserInfo(user.UserInfoId);
            userInfo.FavoritePushNotification = model.turnOn;
            UserManager2.UpdateInfo(userInfo);

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

        private TokenModel GetToken(string email, string password)
        {
            var client = new HttpClient();
            var authorizationHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes("xyz:secretKey"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationHeader);

            var form = new Dictionary<string, string>
               {
                   {"grant_type", "password"},
                   {"username", email},
                   {"password", password}
               };

            var url = ConfigurationManager.AppSettings["tokenUrl"];
            var tokenResponse = client.PostAsync(url, new FormUrlEncodedContent(form)).Result;
            var token = tokenResponse.Content.ReadAsAsync<TokenModel>(new[] { new JsonMediaTypeFormatter() }).Result;

            return token;
        }

        #region OBSOLETE
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
        #endregion

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
