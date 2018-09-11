using System;
using System.Collections.Generic;
using System.Text;

namespace TodoData.Models.Shared
{
    public class SharedTasks
    {
        public virtual long Id { get; set; }

        public virtual long TaskId { get; set; }

        public virtual long UserId { get; set; }

        public virtual int ShareType { get; set; }

        public virtual DateTime LastUpdate { get; set; }

        public virtual bool IsActive { get; set; }
    }
}
