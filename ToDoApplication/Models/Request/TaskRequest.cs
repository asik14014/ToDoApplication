using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApplication.Models.Request
{
    public class TaskRequest
    {
        public long id { get; set; }

        public long groupId { get; set; }

        public long userId { get; set; }

        public int status { get; set; }

        public string name { get; set; }

        public string text { get; set; }

        public long deadline { get; set; }

        public string repeatType { get; set; }

        public long repeatTime { get; set; }

        public long remind { get; set; }
    }
}
