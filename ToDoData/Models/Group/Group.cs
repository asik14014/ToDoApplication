using System;
using System.Collections.Generic;
using System.Text;

namespace TodoData.Models.Group
{
    public class Group
    {
        public virtual long Id { get; set; }

        public virtual long UserId { get; set; }

        public virtual int GroupType { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual int Order { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public virtual DateTime LastUpdate { get; set; }

        public virtual bool IsActive { get; set; }
    }
}
