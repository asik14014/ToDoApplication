using TodoData.Dao;
using TodoData.Models.User;

namespace ToDoApplication.Code
{
    public static class UserManager
    {
        private static UserDaoManager userDaoManager = new UserDaoManager();

        public static User FindUser(string email, string password)
        {
            return userDaoManager.FindUser(email, password);
        }

        public static User Register(User user)
        {
            return userDaoManager.Save(user);
        }

        public static User FindUserInfo(string email)
        {
            return userDaoManager.FindUser(email);
        }

        public static User Update(User user)
        {
            return userDaoManager.SaveOrUpdate(user);
        }
    }
}
