using Microsoft.AspNet.Identity;

namespace ToDoApplication.Models.Identity
{
    public class UserManager : UserManager<TodoData.Models.User.User, long>
    {
        public UserManager(IUserStore<TodoData.Models.User.User, long> store)
            : base(store)
        {
            UserValidator = new UserValidator<TodoData.Models.User.User, long>(this) { AllowOnlyAlphanumericUserNames = false };
            PasswordValidator = new PasswordValidator();
        }
    }
}