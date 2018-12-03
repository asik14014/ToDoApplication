using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoData.Dao;
using TodoData.Models.Task;

namespace TodoData.Dao
{
    public class SubtaskDaoManager: BaseDaoManager<Subtask>
    {
        public Subtask Find(long taskId, long subtaskId)
        {
            try
            {
                using (var session = NHibertnateSession.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        return session.QueryOver<Subtask>()
                            .Where(st => st.SubtaskId == subtaskId && st.TaskId == taskId)
                            .SingleOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<Subtask> GetAllByTaskId(long taskId)
        {
            try
            {
                using (var session = NHibertnateSession.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        return session.QueryOver<Subtask>()
                            .Where(st => st.TaskId == taskId)
                            .List();
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
