using NLog;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http;
using System.Net;
using ToDoApplication.Code;
using System;
using TodoData.Models.Attachment;

namespace ToDoApplication.Controllers
{
    [System.Web.Mvc.RoutePrefix("api/File")]
    [System.Web.Mvc.AllowAnonymous]
    //[Authorize]
    public class FileController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [System.Web.Mvc.HttpPost]
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
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"') + DateTime.Now.ToString("HH:mm:SS");
                var buffer = await file.ReadAsByteArrayAsync();
                //Do whatever you want with filename and its binary data.

                var result = fileManager.UploadFileAsync(buffer, filename);//pass file stream

                if (!string.IsNullOrEmpty(result.Result))
                {
                    return BadRequest(result.Result);
                }

                //save attachment
                var attachment = new Attachment()
                {
                    FileName = filename,
                    FileUrl = filename,
                    FileType = 0,
                    TaskId = taskId,
                    LastUpdate = DateTime.Now
                };
                FileManager.SaveAttachment(attachment);
            }

            return Ok();
        }

        [System.Web.Mvc.HttpGet]
        public async Task<IHttpActionResult> Download(long taskId)
        {
            //TODO прописать скачивание файла
            return Ok();
        }
    }
}
