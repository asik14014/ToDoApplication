using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Cfg;
using TodoData.Models.User;
using ToDoData;

namespace TodoData
{
    public class NHibertnateSession
    {
        public static ISession OpenSession()
        {
            var configuration = new Configuration();
            configuration.Configure();
            ISessionFactory sessionFactory = configuration.BuildSessionFactory();
            return sessionFactory.OpenSession();
        }

        public IUserStore<User, long> Users
        {
            get { return new IdentityStore(OpenSession()); }
        }
    }
}
