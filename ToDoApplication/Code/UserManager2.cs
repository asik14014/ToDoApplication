using System;
using System.Collections.Generic;
using System.Linq;
using ToDoApplication.Models;
using TodoData.Dao;
using TodoData.Models.User;

namespace ToDoApplication.Code
{
    public static class UserManager2
    {
        private static UserDaoManager userDaoManager = new UserDaoManager();
        private static UserInfoDaoManager userInfoDaoManager = new UserInfoDaoManager();
        private static GroupDaoManager groupDaoManager = new GroupDaoManager();

        public static User FindUser(string email, string password)
        {
            return userDaoManager.FindUser(email, password);
        }

        public static User Create(User user)
        {
            return userDaoManager.SaveOrUpdate(user);
        }

        public static User Register(User user)
        {
            var temp = FindUser(user.UserName);
            if (temp != null) throw new Exception($"User already registered for email: {user.UserName}");

            var result = userInfoDaoManager.Save(new UserInfo()
            {
                PhotoUrl = string.Empty,
                Name = string.Empty,
                PhoneNumber = string.Empty,
                RawData = string.Empty
            });
            user.UserInfoId = result.Id;

            return userDaoManager.Save(user);
        }

        public static User FindUser(string email)
        {
            return userDaoManager.FindUser(email);
        }

        public static UserModel Update(UserModel user)
        {
            var temp = userDaoManager.GetById(user.id);
            temp.UserName = user.email;
            var result = userDaoManager.SaveOrUpdate(temp);

            return user;
        }

        public static UserModel GetUserModel(long userId)
        {
            var temp = userDaoManager.GetById(userId);
            var info = userInfoDaoManager.GetById(temp.UserInfoId);
            var groups = groupDaoManager.GetAllByUserId(userId);

            var result = new UserModel()
            {
                id = temp.Id,
                email = temp.UserName,
                username = info.Name,
                avatar = info.PhotoUrl,
                push = info.Push,
                lists = groups != null ? groups.Select(g => new InfoListModel() { id = g.Id, title = g.Name }).ToList() : new List<InfoListModel>() { }
            };

            return result;
        }

        public static UserModel GetUserModel(string email)
        {
            var temp = userDaoManager.FindUser(email);
            var info = userInfoDaoManager.GetById(temp.UserInfoId);

            var result = new UserModel()
            {
                id = temp.Id,
                email = temp.UserName,
                username = info.Name,
                avatar = info.PhotoUrl,
                push = true,
                lists = new List<InfoListModel>() { }
            };

            return result;
        }

        public static User Update(User user)
        {
            var temp = userDaoManager.GetById(user.Id);
            temp.UserName = user.UserName;
            temp.PasswordHash = user.PasswordHash;
            var result = userDaoManager.SaveOrUpdate(temp);

            return user;
        }

        public static UserInfo FindUserInfo(long id)
        {
            return userInfoDaoManager.GetById(id);
        }

        public static UserInfo CreateUserInfo(string photoUrl, string firstName, string lastName, string phoneNumber, string rawData)
        {
            var result = userInfoDaoManager.Save(new UserInfo()
            {
                PhotoUrl = photoUrl,
                Name = $"{firstName} {lastName}",
                PhoneNumber = phoneNumber,
                RawData = rawData
            });

            return result;
        }

        public static UserInfo UpdateInfo(UserInfo info)
        {
            return userInfoDaoManager.SaveOrUpdate(info);
        }
    }
}
