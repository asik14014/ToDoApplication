using NHibernate;
using System;
using System.Collections.Generic;
using TodoData.Models.Group;

namespace TodoData.Dao
{
    public class GroupDaoManager : BaseDaoManager<Group>
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

        public IList<Group> GetByType(long userId, int type)
        {
            try
            {
                using (var session = NHibertnateSession.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        return session.QueryOver<Group>()
                            .Where(g => g.UserId == userId && g.GroupType == type)
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
