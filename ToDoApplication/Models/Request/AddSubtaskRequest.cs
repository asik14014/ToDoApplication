using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoApplication.Models.Request
{
    public class AddSubtaskRequest
    {
        public string title { get; set; }
        public bool status { get; set; }
    }
}