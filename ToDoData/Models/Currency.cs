using System;
using System.Collections.Generic;
using System.Text;

namespace TodoData.Models
{
    public class Currency
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual int Code { get; set; }
    }
}
