using System;
using TodoData.Dao;
using TodoData.Models.Shared;

namespace ToDoApplication.Code
{
    public static class ShareManager
    {
        private static TaskDaoManager taskDaoManager = new TaskDaoManager();
        private static GroupDaoManager groupDaoManager = new GroupDaoManager();
        private static SharedGroupDaoManager sharedGroupDaoManager = new SharedGroupDaoManager();
        private static SharedTaskDaoManager sharedTaskDaoManager = new SharedTaskDaoManager();
        private static ShareTypeDaoManager shareTypeDaoManager = new ShareTypeDaoManager();

        
        public static SharedTasks ShareTask(long taskId, long userId, int type)
        {
            var sharedTask = sharedTaskDaoManager.Find(userId, taskId);

            if (sharedTask == null)
            {
                var entity = new SharedTasks()
                {
                    Id = 0,
                    TaskId = taskId,
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

        public static SharedGroups ShareGroup(long groupId, long userId, int type)
        {
            var sharedGroup = sharedGroupDaoManager.Find(userId, groupId);

            if (sharedGroup == null)
            {
                var entity = new SharedGroups()
                {
                    Id = 0,
                    GroupId = groupId,
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

        public static bool UnshareTask(long taskId, long userId)
        {
            var sharedTask = sharedTaskDaoManager.Find(userId, taskId);

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

        public static bool UnshareGroup(long groupId, long userId)
        {
            var sharedGroup = sharedGroupDaoManager.Find(userId, groupId);

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
