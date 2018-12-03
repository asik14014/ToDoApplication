using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoData.Models.Task
{
    public class Subtask
    {
        public virtual long Id { get; set; }
        public virtual long TaskId { get; set; }
        public virtual long SubtaskId { get; set; }
        public virtual DateTime Registration { get; set; }
    }
}
