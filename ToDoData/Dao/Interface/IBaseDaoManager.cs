using System;
using System.Collections.Generic;

namespace TodoData.Dao.Interface
{
    public interface IBaseDaoManager<T>
    {
        T GetById(object id, bool shouldLock);

        List<T> GetAll();

        List<T> GetByExample(T exampleInstance, string[] propertiesToExclude);

        T SaveOrUpdate(T entity);

        void Delete(T entity);
    }
}
