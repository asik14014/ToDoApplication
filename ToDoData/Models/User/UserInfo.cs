using System;
using System.Collections.Generic;
using System.Text;

namespace TodoData.Models.User
{
    public class UserInfo
    {
        public virtual long Id { get; set; }

        public virtual string PhotoUrl { get; set; }

        public virtual string Name { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual string RawData { get; set; }
    }
}
