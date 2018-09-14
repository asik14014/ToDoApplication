using System.Collections.Generic;
using TodoData.Dao;
using TodoData.Models.Shared;
using TodoData.Models.Task;

namespace ToDoApplication.Code
{
    public static class TaskManager
    {
        private static TaskDaoManager taskDaoManager = new TaskDaoManager();
        private static SharedTaskDaoManager sharedTaskDaoManager = new SharedTaskDaoManager();

        public static List<Task> GetAllTasks()
        {
            return taskDaoManager.GetAll();
        }

        public static IList<Task> GetAllTasksByUser(long userId)
        {
            return taskDaoManager.GetAllByUserId(userId);
        }

        public static IList<SharedTasks> GetAllSharedTasksByUser(long userId)
        {
            return sharedTaskDaoManager.GetAllByUserId(userId);
        }

        public static IList<Task> GetAllTasksByGroup(long groupId)
        {
            return taskDaoManager.GetAllByGroupId(groupId);
        }

        public static Task Create(Task task)
        {
            return taskDaoManager.Save(task);
        }

        public static void Delete(Task task)
        {
            taskDaoManager.Delete(task);
        }

        public static Task GetTask(long id)
        {
            return taskDaoManager.GetById(id);
        }

        public static Task SaveOrUpdate(Task task)
        {
            return taskDaoManager.SaveOrUpdate(task);
        }
    }
}
