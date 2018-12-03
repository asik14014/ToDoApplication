using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoApplication.Models.Request
{
    public class AddUserRequest
    {
        public long taskId { get; set; }

        public long id { get; set; }

        public string name { get; set; }
    }
}