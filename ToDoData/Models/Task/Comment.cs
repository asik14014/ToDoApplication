using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoData.Models.Task
{
    public class Comment
    {
        public virtual long Id { get; set; }
        public virtual long UserId { get; set; }
        public virtual long TaskId { get; set; }
        public virtual string Text { get; set; }
        public virtual DateTime LastUpdate { get; set; }
    }
}
