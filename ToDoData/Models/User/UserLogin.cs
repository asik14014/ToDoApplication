using Microsoft.AspNet.Identity;
using System;

namespace TodoData.Models.User
{
    public class UserLogin
    {
        public virtual long Id { get; set; }
        public virtual string LoginProvider { get; set; }
        public virtual string ProviderKey { get; set; }
        public virtual long UserId { get; set; }
    }
}
