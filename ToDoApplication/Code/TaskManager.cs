using System;
using System.Collections.Generic;
using ToDoApplication.Models.Request;
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

        public static void Delete(long taskId)
        {
            var entity = taskDaoManager.GetById(taskId);
            taskDaoManager.Delete(entity);
        }

        public static Task GetTask(long id)
        {
            return taskDaoManager.GetById(id);
        }

        public static Task SaveOrUpdate(TaskRequest task)
        {
            var entity = new Task()
            {
                GroupId = task.GroupId,
                UserId = task.UserId,
                Status = task.Status,
                CreationDate = DateTime.Now,
                LastUpdate = DateTime.Now
            };
            return taskDaoManager.SaveOrUpdate(entity);
        }

        public static Task Save(TaskRequest task)
        {
            var entity = new Task()
            {
                GroupId = task.GroupId,
                UserId = task.UserId,
                Status = task.Status,
                CreationDate = DateTime.Now,
                LastUpdate = DateTime.Now
            };
            return taskDaoManager.Save(entity);
        }

        public static Task Update(TaskRequest task)
        {
            var entity = new Task()
            {
                Id = task.Id,
                GroupId = task.GroupId,
                UserId = task.UserId,
                Status = task.Status,
                CreationDate = DateTime.Now,
                LastUpdate = DateTime.Now
            };
            return taskDaoManager.Update(entity);
        }
    }
}
