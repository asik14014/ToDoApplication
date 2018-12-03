using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoApplication.Models.Request
{
    public class ShortGroupRequest
    {
        public virtual int groupType { get; set; }

        public virtual string name { get; set; }

        public virtual string description { get; set; }

        public virtual int order { get; set; }
    }
}