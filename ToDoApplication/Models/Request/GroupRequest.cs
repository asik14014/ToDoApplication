﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApplication.Models.Request
{
    public class GroupRequest
    {
        public virtual long id { get; set; }

        public virtual long userId { get; set; }

        public virtual int groupType { get; set; }

        public virtual string name { get; set; }

        public virtual string description { get; set; }

        public virtual int order { get; set; }
    }
}
