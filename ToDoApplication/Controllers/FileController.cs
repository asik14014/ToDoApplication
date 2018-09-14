using System;
using NLog;
using ToDoApplication.Code;
using ToDoApplication.Models;
using TodoData.Models.Group;
using System.Web.Mvc;
using ToDoApplication.Models.Request;
using System.Threading.Tasks;
using System.Net.Http;

namespace ToDoApplication.Controllers
{
    [RoutePrefix("api/Group")]
    [AllowAnonymous]
    //[Authorize]
    public class FileController : Controller
    {
        /*
        private static Logger logger = LogManager.GetCurrentClassLogger();
        [HttpPost]
        public async Task<System.Web.Http.IHttpActionResult> Upload()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                var buffer = await file.ReadAsByteArrayAsync();
                //Do whatever you want with filename and its binary data.
            }

            return Ok();
        }*/
    }
}
