using NLog;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using ToDoApplication.Code;
using ToDoApplication.Models;
using TodoData.Models.User;

namespace ToDoApplication.Controllers
{
    [System.Web.Mvc.RoutePrefix("api/User")]
    [System.Web.Mvc.AllowAnonymous]
    //[Authorize]
    public class UserController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        public object Registration(User user)
        {
            logger.Log(LogLevel.Debug, $"UserController.Registration({Json(user)})");
            try
            {
                var result = UserManager2.Register(user);

                try
                {
                    GroupManager.CreateFavorites(result.Id);
                }
                catch (Exception e)
                {
                    logger.Log(LogLevel.Error, $"Не смог создать группу избранное." +
                        $"UserController.Registration({Json(user)}) - {e}");
                }

                return new HttpStatusCodeResult(200);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"UserController.Registration({Json(user)}) - {ex}");
                //изменить http status code
                return new Response(100, ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>'
        [System.Web.Mvc.HttpGet]
        public object GetToken(string login, string password)
        {
            return "";
        }

        [System.Web.Mvc.HttpGet]
        public object Find(string login)
        {
            logger.Log(LogLevel.Debug, $"UserController.Find({login})");
            try
            {
                return UserManager2.FindUserInfo(login);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"UserController.Find({login}) - {ex}");
                //изменить http status code
                return new Response(100, ex.Message);
            }
        }

        [System.Web.Mvc.HttpPut]
        public object Update(User user)
        {
            logger.Log(LogLevel.Debug, $"UserController.Update({Json(user)})");
            try
            {
                return UserManager2.Update(user);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"UserController.Update({Json(user)}) - {ex}");
                //изменить http status code
                return new Response(100, ex.Message);
            }
        }

        [System.Web.Mvc.HttpPost]
        public async Task<System.Web.Http.IHttpActionResult> UpdateImage()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var user = User.Identity.Name;

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            var fileManager = new FileManager();
            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                var buffer = await file.ReadAsByteArrayAsync();
                //Do whatever you want with filename and its binary data.

                var result = fileManager.UploadFileAsync(buffer, $"{user}.png");//pass file stream

                if (!string.IsNullOrEmpty(result.Result))
                {
                    return BadRequest(result.Result);
                }
            }

            return Ok();
        }

        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetImage()
        {
            var stream = new MemoryStream();
            // processing the stream.

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(stream.ToArray())
            };

            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = "CertificationCard.pdf" // profile photo path
            };

            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return result;
        }


        /*
        private System.Web.Http.IHttpActionResult GetErrorResult(IdentityResult result)
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
        }*/
    }
}