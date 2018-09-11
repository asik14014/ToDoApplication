using NHibernate;
using System;
using System.Collections.Generic;
using System.Text;
using TodoData.Models.Shared;

namespace TodoData.Dao
{
    public class SharedTaskDaoManager : BaseDaoManager<SharedTasks>
    {
        public IList<SharedTasks> GetAllByUserId(long userId)
        {
            try
            {
                using (var session = NHibertnateSession.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        return session.QueryOver<SharedTasks>()
                            .Where(st => st.UserId == userId && st.IsActive)
                            .List();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SharedTasks Find(long userId, long taskId)
        {
            try
            {
                using (var session = NHibertnateSession.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        return session.QueryOver<SharedTasks>()
                            .Where(st => st.UserId == userId && st.TaskId == taskId)
                            .SingleOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SharedTasks Update(SharedTasks entity)
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
