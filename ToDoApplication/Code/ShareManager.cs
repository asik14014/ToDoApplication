using System;
using TodoData.Dao;
using TodoData.Models.Group;
using TodoData.Models.Shared;
using TodoData.Models.Task;

namespace ToDoApplication.Code
{
    public static class ShareManager
    {
        private static TaskDaoManager taskDaoManager = new TaskDaoManager();
        private static GroupDaoManager groupDaoManager = new GroupDaoManager();
        private static SharedGroupDaoManager sharedGroupDaoManager = new SharedGroupDaoManager();
        private static SharedTaskDaoManager sharedTaskDaoManager = new SharedTaskDaoManager();
        private static ShareTypeDaoManager shareTypeDaoManager = new ShareTypeDaoManager();

        public static SharedTasks ShareTask(Task task, long userId, int type)
        {
            var sharedTask = sharedTaskDaoManager.Find(userId, task.Id);

            if (sharedTask == null)
            {
                var entity = new SharedTasks()
                {
                    Id = 0,
                    TaskId = task.Id,
                    UserId = userId,
                    ShareType = type,
                    IsActive = true,
                    LastUpdate = DateTime.Now
                };
                sharedTaskDaoManager.Save(entity);

                return entity;
            }
            else if (!sharedTask.IsActive)
            {
                sharedTask.IsActive = true;
                return sharedTaskDaoManager.Update(sharedTask);
            }

            return sharedTask;
        }

        public static SharedGroups ShareGroup(Group group, long userId, int type)
        {
            var sharedGroup = sharedGroupDaoManager.Find(userId, group.Id);

            if (sharedGroup == null)
            {
                var entity = new SharedGroups()
                {
                    Id = 0,
                    GroupId = group.Id,
                    UserId = userId,
                    ShareType = type,
                    IsActive = true,
                    LastUpdate = DateTime.Now
                };
                sharedGroupDaoManager.Save(entity);

                return entity;
            }
            else if (!sharedGroup.IsActive)
            {
                sharedGroup.IsActive = true;
                return sharedGroupDaoManager.Update(sharedGroup);
            }

            return sharedGroup;
        }

        public static bool UnshareTask(Task task, long userId)
        {
            var sharedTask = sharedTaskDaoManager.Find(userId, task.Id);

            if (sharedTask == null)
            {
                return true;
            }
            else if (sharedTask.IsActive)
            {
                sharedTask.IsActive = false;
                sharedTaskDaoManager.Update(sharedTask);
            }
            return true;
        }

        public static bool UnshareGroup(Group group, long userId)
        {
            var sharedGroup = sharedGroupDaoManager.Find(userId, group.Id);

            if (sharedGroup == null)
            {
                return true;
            }
            else if (sharedGroup.IsActive)
            {
                sharedGroup.IsActive = false;
                sharedGroupDaoManager.Update(sharedGroup);
            }
            return true;
        }
    }
}
