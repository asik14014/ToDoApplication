using System;
using Microsoft.AspNet.Identity;
using TodoData.Models.User;

namespace ToDoApplication.Models.Identity
{
    public class UserManager : UserManager<User, long>
    {
        public UserManager(IUserStore<User, long> store)
            : base(store)
        {
            UserValidator = new UserValidator<User, long>(this);
            PasswordValidator = new PasswordValidator();
        }
    }
}