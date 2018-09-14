using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApplication.Models.Request
{
    public class GroupRequest
    {
        public virtual long Id { get; set; }

        public virtual long UserId { get; set; }

        public virtual int GroupType { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual int Order { get; set; }
    }
}
