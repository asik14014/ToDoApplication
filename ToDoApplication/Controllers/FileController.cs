using NLog;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http;
using System.Net;
using ToDoApplication.Code;
using System;
using TodoData.Models.Attachment;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using Microsoft.AspNet.Identity;
using ToDoApplication.Models;

namespace ToDoApplication.Controllers
{
    [RoutePrefix("api/File")]
    [Authorize]
    public class FileController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Upload")]
        public async Task<IHttpActionResult> Upload(long taskId)
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var task = TaskManager.GetTask(taskId);
            if (task == null) return NotFound();

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            //var fileManager = new AwsFileManager(); //amazon
            var fileManager = new AzureFileManager();
            foreach (var file in provider.Contents)
            {
                //var filename = DateTime.Now.ToString("HHmmSS") + file.Headers.ContentDisposition.FileName.Trim('\"');
                var filename = "asdsad";
                var buffer = await file.ReadAsByteArrayAsync();
                //var filename = model.file.FileName/*file.Headers.ContentDisposition.FileName*/.Trim('\"') + DateTime.Now.ToString("HH:mm:SS");
                //var buffer = ReadFully(model.file.InputStream);//await file.ReadAsByteArrayAsync();
                //Do whatever you want with filename and its binary data.

                var result = fileManager.UploadFileAsync(buffer, filename);//pass file stream

                if (string.IsNullOrEmpty(result))
                {
                    return BadRequest(result);
                }

                //save attachment
                var attachment = new Attachment()
                {
                    FileName = filename,
                    FileUrl = result,
                    FileType = 0,
                    TaskId = taskId,
                    LastUpdate = DateTime.Now
                };
                FileManager.SaveAttachment(attachment);
            }

            return Ok();
        }

        [HttpGet]
        [Route("Download")]
        public async Task<IHttpActionResult> Download(long taskId)
        {
            //TODO прописать скачивание файла
            return Ok();
        }

        [HttpGet]
        public HttpResponseMessage DownloadFile(int taskId, string filename)
        {
            try
            {
                var stream = new MemoryStream();
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(stream.ToArray())
                };

                //TODO поменять на url 
                var attachments = FileManager.GetAttachments(taskId);
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = attachments.First().FileName
                };

                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                return result;
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}
