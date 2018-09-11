using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TodoData.Models.Group;

namespace TodoData.Dao
{
    public class GroupDaoManager: BaseDaoManager<Group>
    {
        /// <summary>
        /// Вытащить все группы по пользователю
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IList<Group> GetAllByUserId(long userId)
        {
            try
            {
                using (var session = NHibertnateSession.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        return session.QueryOver<Group>()
                            .Where(g => g.UserId == userId && g.IsActive)
                            .OrderBy(g => g.Order)
                            .Asc.List();
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
