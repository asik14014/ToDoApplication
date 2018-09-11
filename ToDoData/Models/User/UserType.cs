using System;
using System.Collections.Generic;
using System.Text;

namespace TodoData.Models.User
{
    public class UserType
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }
    }
}
