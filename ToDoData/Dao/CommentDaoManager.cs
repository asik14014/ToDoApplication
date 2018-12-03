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
    public class CommentDaoManager : BaseDaoManager<Comment>
    {
        public IList<Comment> GetAllByTaskId(long taskId)
        {
            try
            {
                using (var session = NHibertnateSession.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        return session.QueryOver<Comment>()
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
