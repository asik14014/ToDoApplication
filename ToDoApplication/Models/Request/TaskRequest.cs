using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApplication.Models.Request
{
    public class TaskRequest
    {
        public long Id { get; set; }

        public long GroupId { get; set; }

        public long UserId { get; set; }

        public int Status { get; set; }
    }
}
