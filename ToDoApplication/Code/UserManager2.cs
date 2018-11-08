﻿using System;
using TodoData.Dao;
using TodoData.Models.User;

namespace ToDoApplication.Code
{
    public static class UserManager2
    {
        private static UserDaoManager userDaoManager = new UserDaoManager();
        private static UserInfoDaoManager userInfoDaoManager = new UserInfoDaoManager();

        public static User FindUser(string email, string password)
        {
            return userDaoManager.FindUser(email, password);
        }

        public static User Register(User user)
        {
            var temp = FindUserInfo(user.UserName);
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

        public static User FindUserInfo(string email)
        {
            return userDaoManager.FindUser(email);
        }

        public static User Update(User user)
        {
            return userDaoManager.SaveOrUpdate(user);
        }
    }
}
