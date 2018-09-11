using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoApplication.Models
{
    public class Response
    {
        public Response()
        { }

        public Response(int code, string description)
        {
            Code = code;
            Description = description;
        }

        public int Code;

        public string Description;
    }
}