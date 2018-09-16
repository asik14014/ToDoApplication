using System;
using Microsoft.AspNet.Identity;

namespace TodoData.Models.User
{
    public class User : IUser<long>
    {
        public virtual long Id { get; protected set; }

        public virtual string UserName { get; set; }

        public virtual string PasswordHash { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual int UserType { get; set; }

        public virtual long UserInfoId { get; set; }

        public virtual int AccountPlanId { get; set; }

        public virtual DateTime Registration { get; set; }

        public virtual DateTime LastUpdate { get; set; }
    }
}
