﻿using NLog;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using ToDoApplication.Code;

namespace ToDoApplication.Controllers
{
    [System.Web.Mvc.RoutePrefix("api/File")]
    [System.Web.Mvc.AllowAnonymous]
    //[Authorize]
    public class FileController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [System.Web.Mvc.HttpPost]
        public async Task<IHttpActionResult> Upload()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            var fileManager = new FileManager();
            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                var buffer = await file.ReadAsByteArrayAsync();
                //Do whatever you want with filename and its binary data.

                var result = fileManager.UploadFileAsync(buffer, "test.png");//pass file stream

                if (!string.IsNullOrEmpty(result.Result))
                {
                    return BadRequest(result.Result);
                }
            }

            return Ok();
        }

        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage Download(int taskId)
        {
            var stream = new MemoryStream();
            // processing the stream.

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(stream.ToArray())
            };

            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                //TODO поменять на url 
                FileName = "CertificationCard.pdf" // profile photo path
            };

            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return result;
        }
    }
}
