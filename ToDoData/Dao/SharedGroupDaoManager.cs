using NHibernate;
using System;
using System.Collections.Generic;
using System.Text;
using TodoData.Models.Shared;

namespace TodoData.Dao
{
    public class SharedGroupDaoManager : BaseDaoManager<SharedGroups>
    {
        public IList<SharedGroups> GetAllByUserId(long userId)
        {
            try
            {
                using (var session = NHibertnateSession.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        return session.QueryOver<SharedGroups>()
                            .Where(sg => sg.UserId == userId && sg.IsActive)
                            .List();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SharedGroups Find(long userId, long groupId)
        {
            try
            {
                using (var session = NHibertnateSession.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        return session.QueryOver<SharedGroups>()
                            .Where(sg => sg.UserId == userId && sg.GroupId == groupId)
                            .SingleOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SharedGroups Update(SharedGroups entity)
        {
            try
            {
                using (var session = NHibertnateSession.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        try
                        {
                            session.Update(entity);
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            if (!transaction.WasCommitted) transaction.Rollback();
                            throw;
                        }
                    }
                }

                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
