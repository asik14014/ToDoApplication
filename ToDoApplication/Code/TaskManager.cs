using System;
using System.Collections.Generic;
using System.Linq;
using ToDoApplication.Models;
using ToDoApplication.Models.Request;
using TodoData.Dao;
using TodoData.Models.Shared;
using TodoData.Models.Task;

namespace ToDoApplication.Code
{
    public static class TaskManager
    {
        private static TaskDaoManager taskDaoManager = new TaskDaoManager();
        private static AttachmentDaoManager attachmentDaoManager = new AttachmentDaoManager();
        private static SharedTaskDaoManager sharedTaskDaoManager = new SharedTaskDaoManager();
        private static UserDaoManager userDaoManager = new UserDaoManager();

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

        public static TaskModel GetTask(long id)
        {
            var item = taskDaoManager.GetById(id);
            if (item == null) return null;

            var result = new TaskModel()
            {
                id = item.Id,
                title = item.Name,
                deadline = item.EndDate.HasValue ? item.EndDate.Value.ToLongDateString() : "0",
                description = item.Description,
                repeat = new IterationModule()
                {
                    repeatEvery = 0,
                    type = "NEVER"
                },
                remind = 0
            };
            var files = attachmentDaoManager.GetAllByTaskId(id);
            result.files = (files != null) ? files.Select(f => new FileModel() { name = f.FileName, link = f.FileUrl }).ToList() : null;

            var temp = sharedTaskDaoManager.GetAllByTaskId(id);
            var sharedUsers = temp.Select(t => t.UserId);
            var users = new List<UserListModel>();

            if (sharedUsers != null)
            {
                foreach (var userId in sharedUsers)
                {
                    var user = userDaoManager.GetById(userId);
                    users.Add(new UserListModel()
                    {
                        id = user.Id,
                        name = user.UserName
                    });
                }
                result.users = users;
            }

            return result;
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
