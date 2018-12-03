using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoApplication.Models.Request
{
    public class AddCommentRequest
    {
        public long taskId { get; set; }

        public string text { get; set; }
    }
}