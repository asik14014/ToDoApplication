using System;

namespace TodoData.Models.Task
{
    public class Task
    {
        public virtual long Id { get; set; }

        public virtual long GroupId { get; set; }

        public virtual long UserId { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public virtual int Status { get; set; }

        public virtual DateTime LastUpdate { get; set; }

        public virtual DateTime? EndDate { get; set; }
    }
}
