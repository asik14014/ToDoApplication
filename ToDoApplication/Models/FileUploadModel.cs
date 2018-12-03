using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoApplication.Models
{
    public class FileUploadModel
    {
        public HttpPostedFileBase file { get; set; }
        public long taskId { get; set; }
    }
}