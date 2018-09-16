using NHibernate;
using System;
using System.Collections.Generic;
using System.Text;
using TodoData.Models.User;
using System.Linq;

namespace TodoData.Dao
{
    public class UserDaoManager : BaseDaoManager<User>
    {
        public User FindUser(string email, string password)
        {
            try
            {
                using (var session = NHibertnateSession.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        return session.QueryOver<User>()
                            .Where(t => t.UserName == email && t.PasswordHash == password)
                            .SingleOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public User FindUser(string email)
        {
            try
            {
                using (var session = NHibertnateSession.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        return session.QueryOver<User>()
                            .Where(t => t.UserName == email)
                            .SingleOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
