using System;
using System.Collections.Generic;
using System.Text;

namespace TodoData.Models.Shared
{
    public class SharedGroups
    {
        public virtual long Id { get; set; }

        public virtual long GroupId { get; set; }

        public virtual long UserId { get; set; }

        public virtual int ShareType { get; set; }

        public virtual DateTime LastUpdate { get; set; }

        public virtual bool IsActive { get; set; }
    }
}
