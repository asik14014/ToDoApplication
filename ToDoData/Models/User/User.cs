using System;

namespace TodoData.Models.User
{
    public class User
    {
        public virtual long Id { get; set; }

        public virtual string Email { get; set; }

        public virtual string Password { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual int UserType { get; set; }

        public virtual long UserInfoId { get; set; }

        public virtual int AccountPlanId { get; set; }

        public virtual DateTime Registration { get; set; }

        public virtual DateTime LastUpdate { get; set; }
    }
}
