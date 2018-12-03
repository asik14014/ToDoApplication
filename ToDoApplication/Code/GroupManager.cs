using System;
using System.Collections.Generic;
using ToDoApplication.Models.Request;
using TodoData.Dao;
using TodoData.Enum;
using TodoData.Models.Group;
using TodoData.Models.Shared;

namespace ToDoApplication.Code
{
    public static class GroupManager
    {
        private static GroupDaoManager groupDaoManager = new GroupDaoManager();
        private static SharedGroupDaoManager sharedGroupDaoManager = new SharedGroupDaoManager();

        public static List<Group> GetAllGroups()
        {
            return groupDaoManager.GetAll();
        }

        public static IList<Group> GetAllGroupsByUser(long userId)
        {
            return groupDaoManager.GetAllByUserId(userId);
        }

        public static IList<SharedGroups> GetAllSharedGroupsByUser(long userId)
        {
            return sharedGroupDaoManager.GetAllByUserId(userId);
        }

        public static Group Create(Group group)
        {
            return groupDaoManager.Save(group);
        }

        public static void Delete(long groupId)
        {
            var group = groupDaoManager.GetById(groupId);
            groupDaoManager.Delete(group);
        }

        public static Group GetGroup(long id)
        {
            return groupDaoManager.GetById(id);
        }

        public static Group SaveOrUpdate(Group group)
        {
            return groupDaoManager.SaveOrUpdate(group);
        }

        public static Group Save(GroupRequest group)
        {
            var entity = new Group()
            {
                UserId = group.userId,
                GroupType = group.groupType,
                Name = group.name,
                Description = group.description,
                Order = group.order,
                CreationDate = DateTime.Now,
                LastUpdate = DateTime.Now,
                IsActive = true
            };
            return groupDaoManager.Save(entity);
        }

        public static Group Update(GroupRequest group)
        {
            var entity = new Group()
            {
                Id = group.id,
                UserId = group.userId,
                GroupType = group.groupType,
                Name = group.name,
                Description = group.description,
                Order = group.order,
                CreationDate = DateTime.Now,
                LastUpdate = DateTime.Now,
                IsActive = true
            };
            return groupDaoManager.Update(entity);
        }

        public static Group CreateFavorites(long userId)
        {
            var group = new Group()
            {
                Id = 0,
                UserId = userId,
                GroupType = (int)GroupTypeEnum.FAVOURITES,
                Name = "Избранное",
                Description = "Группа избранное",
                Order = 0,
                CreationDate = DateTime.Now,
                LastUpdate = DateTime.Now,
                IsActive = true
            };
            return groupDaoManager.Save(group);
        }
    }
}
