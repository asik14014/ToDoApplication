using System;
using System.Collections.Generic;
using System.Text;

namespace TodoData.Models.Shared
{
    public class ShareType
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual DateTime LastUpdate { get; set; }

        public virtual bool IsActive { get; set; }
    }
}
