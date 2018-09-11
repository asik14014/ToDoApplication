using System;
using System.Collections.Generic;
using System.Text;

namespace TodoData.Models.Task
{
    public class Task
    {
        public virtual long Id { get; set; }

        public virtual long GroupId { get; set; }

        public virtual long UserId { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public virtual int Status { get; set; }

        public virtual DateTime LastUpdate { get; set; }
    }
}
