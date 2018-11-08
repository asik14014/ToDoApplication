using NHibernate;
using System;
using System.Collections.Generic;
using TodoData.Models.Attachment;

namespace TodoData.Dao
{
    public class AttachmentDaoManager : BaseDaoManager<Attachment>
    {
        public IList<Attachment> GetAllByTaskId(long taskId)
        {
            try
            {
                using (var session = NHibertnateSession.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        return session.QueryOver<Attachment>()
                            .Where(t => t.TaskId == taskId).List();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<Attachment> GetAllByTask(long taskId)
        {
            try
            {
                using (var session = NHibertnateSession.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        return session.QueryOver<Attachment>()
                            .Where(t => t.TaskId == taskId)
                            .List();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Attachment GetAllByTaskAndName(long taskId, string name)
        {
            try
            {
                using (var session = NHibertnateSession.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        return session.QueryOver<Attachment>()
                            .Where(t => t.TaskId == taskId && t.FileName == name)
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
