using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoApplication.Models.Request
{
    public class AddSubtaskRequest
    {
        public long taskId { get; set; }

        public long subTaskId { get; set; }
    }
}