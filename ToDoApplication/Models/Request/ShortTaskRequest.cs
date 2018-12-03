using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoApplication.Models.Request
{
    public class ShortTaskRequest
    {
        public long groupId { get; set; }

        public int status { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public int deadline { get; set; }
    }
}