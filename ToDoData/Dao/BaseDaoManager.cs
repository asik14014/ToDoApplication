using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using TodoData.Dao.Interface;

namespace TodoData.Dao
{
    public class BaseDaoManager<T> : IBaseDaoManager<T>
    {
        //private static Logger logger = LogManager.GetCurrentClassLogger();

        public BaseDaoManager()
        {
        }

        public void Delete(T entity)
        {
            try
            {
                using (var session = NHibertnateSession.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        try
                        {
                            session.Delete(entity);
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            if (!transaction.WasCommitted) transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<T> GetAll()
        {
            try
            {
                using (var session = NHibertnateSession.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        return session.Query<T>().ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<T> GetByExample(T exampleInstance, string[] propertiesToExclude)
        {
            throw new NotImplementedException();
        }

        public T GetById(object id, bool shouldLock = false)
        {
            try
            {
                using (var session = NHibertnateSession.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        return session.Get<T>(id);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public T Save(T entity)
        {
            try
            {
                using (var session = NHibertnateSession.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        try
                        {
                            session.Save(entity);
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

        public T SaveOrUpdate(T entity)
        {
            try
            {
                using (var session = NHibertnateSession.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        try
                        {
                            session.SaveOrUpdate(entity);
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
