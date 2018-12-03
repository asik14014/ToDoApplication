using Microsoft.AspNet.Identity;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoData.Models.User;
using System.Linq;

namespace ToDoData
{
    public class IdentityStore : IUserStore<User, long>,
        IUserPasswordStore<User, long>,
        IUserLockoutStore<User, long>,
        IUserTwoFactorStore<User, long>,
        IUserLoginStore<User, long>
    {
        private readonly ISession session;

        public IdentityStore(ISession session)
        {
            this.session = session;
        }

        #region IUserStore<User, long>
        public Task CreateAsync(User user)
        {
            return Task.Run(() => session.SaveOrUpdate(user));
        }

        public Task DeleteAsync(User user)
        {
            return Task.Run(() => session.Delete(user));
        }

        public Task<User> FindByIdAsync(long userId)
        {
            return Task.Run(() => session.Get<User>(userId));
        }

        public Task<User> FindByNameAsync(string userName)
        {
            return Task.Run(() =>
            {
                return session.QueryOver<User>()
                    .Where(u => u.UserName == userName)
                    .SingleOrDefault();
            });
        }

        public Task UpdateAsync(User user)
        {
            return Task.Run(() => 
            {
                session.Update(user);
            });
        }
        #endregion

        #region IUserPasswordStore<User, long>
        public Task SetPasswordHashAsync(User user, string passwordHash)
        {
            return Task.Run(() => 
            {
                user.PasswordHash = passwordHash;
                session.Update(user);
            });
        }

        public Task<string> GetPasswordHashAsync(User user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user)
        {
            return Task.FromResult(true);
        }
        
        #endregion

        #region IUserLockoutStore<User, long>
        public Task<DateTimeOffset> GetLockoutEndDateAsync(User user)
        {
            return Task.FromResult(DateTimeOffset.MaxValue);
        }

        public Task SetLockoutEndDateAsync(User user, DateTimeOffset lockoutEnd)
        {
            return Task.CompletedTask;
        }

        public Task<int> IncrementAccessFailedCountAsync(User user)
        {
            return Task.FromResult(0);
        }

        public Task ResetAccessFailedCountAsync(User user)
        {
            return Task.CompletedTask;
        }

        public Task<int> GetAccessFailedCountAsync(User user)
        {
            return Task.FromResult(0);
        }

        public Task<bool> GetLockoutEnabledAsync(User user)
        {
            return Task.FromResult(false);
        }

        public Task SetLockoutEnabledAsync(User user, bool enabled)
        {
            return Task.CompletedTask;
        }
        #endregion

        #region IUserTwoFactorStore<User, int>
        public Task SetTwoFactorEnabledAsync(User user, bool enabled)
        {
            return Task.CompletedTask;
        }

        public Task<bool> GetTwoFactorEnabledAsync(User user)
        {
            return Task.FromResult(false);
        }
        #endregion
        
        public void Dispose()
        {
            //do nothing
        }

        public Task AddLoginAsync(User user, UserLoginInfo login)
        {
            return Task.Run(() => session.SaveOrUpdate(new UserLogin()
            {
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                UserId = user.Id
            }));
        }

        public Task RemoveLoginAsync(User user, UserLoginInfo login)
        {
            return Task.Run(() => session.Delete(new UserLogin()
            {
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                UserId = user.Id
            }));
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(User user)
        {
            return Task.Run(() =>
            {
                return (IList<UserLoginInfo>)session.QueryOver<UserLogin>()
                .Where(u => u.UserId == user.Id).List()
                .Select(i => new UserLoginInfo(i.LoginProvider, i.ProviderKey)).ToList();
            });
        }

        public Task<User> FindAsync(UserLoginInfo login)
        {
            return Task.Run(() =>
            {
                var userLogin = session.QueryOver<UserLogin>()
                    .Where(u => u.LoginProvider == login.LoginProvider && u.ProviderKey == login.ProviderKey)
                    .SingleOrDefault();

                if (userLogin == null) return null;

                return session.QueryOver<User>()
                    .Where(u => u.Id == userLogin.UserId)
                    .SingleOrDefault();
            });
        }
    }
}
