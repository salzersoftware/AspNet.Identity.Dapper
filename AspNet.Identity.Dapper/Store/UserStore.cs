using AspNet.Identity.Dapper.Connection.Interfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNet.Identity.Dapper.Store
{
    /// <summary>
    /// Class that implements the key ASP.NET Identity user store iterfaces
    /// </summary>
    public class UserStore<TUser> : IUserLoginStore<TUser,int>,
        IUserClaimStore<TUser,int>,
        IUserRoleStore<TUser,int>,
        IUserPasswordStore<TUser,int>,
        IUserSecurityStampStore<TUser,int>,
        IQueryableUserStore<TUser,int>,
        IUserEmailStore<TUser,int>,
        IUserPhoneNumberStore<TUser,int>,
        IUserTwoFactorStore<TUser, int>,
        IUserLockoutStore<TUser, int>,
        IUserStore<TUser,int>
        where TUser : IdentityMember
    {
        private UserTable<TUser> UserTable { get; }
        private RoleTable RoleTable { get; }
        private UserRolesTable UserRolesTable { get; }
        private UserClaimsTable UserClaimsTable { get; }
        private UserLoginsTable UserLoginsTable { get; }

        private IDbConnectionFactory DbConnectionFactory { get; }
        public IQueryable<TUser> Users { get { throw new NotImplementedException(); } }

        public UserStore(IDbConnectionFactory dbConnectionFactory)
        {
            DbConnectionFactory = dbConnectionFactory;
            UserTable = new UserTable<TUser>(dbConnectionFactory);
            RoleTable = new RoleTable(dbConnectionFactory);
            UserRolesTable = new UserRolesTable(dbConnectionFactory);
            UserClaimsTable = new UserClaimsTable(dbConnectionFactory);
            UserLoginsTable = new UserLoginsTable(dbConnectionFactory);
        }

        public async Task CreateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            int id = await UserTable.Insert(user);

            // Assign the auto-generated ID to the user.
            user.Id = id;
        }

        public Task<TUser> FindByIdAsync(int userId)
        {
            return UserTable.GetUserById(userId);
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            return UserTable.GetUserByName(userName);
        }

        public Task UpdateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return UserTable.Update(user);
        }

        public void Dispose()
        {
            // Nothing to dispose.
        }

        /// <summary>
        /// Inserts a claim to the UserClaimsTable for the given user
        /// </summary>
        /// <param name="user">User to have claim added</param>
        /// <param name="claim">Claim to be added</param>
        /// <returns></returns>
        public Task AddClaimAsync(TUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            UserClaimsTable.Insert(claim, user.Id);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Returns all claims for a given user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            ClaimsIdentity identity = UserClaimsTable.FindByUserId(user.Id);

            return Task.FromResult<IList<Claim>>(identity.Claims.ToList());
        }

        /// <summary>
        /// Removes a claim froma user
        /// </summary>
        /// <param name="user">User to have claim removed</param>
        /// <param name="claim">Claim to be removed</param>
        /// <returns></returns>
        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            UserClaimsTable.Delete(user, claim);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Inserts a Login in the UserLoginsTable for a given User
        /// </summary>
        /// <param name="user">User to have login added</param>
        /// <param name="login">Login to be added</param>
        /// <returns></returns>
        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            UserLoginsTable.Insert(user, login);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Returns an TUser based on the Login info
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var userId = UserLoginsTable.FindUserIdByLogin(login);
            if (userId > 0)
            {
                TUser user = UserTable.GetUserById(userId) as TUser;
                if (user != null)
                {
                    return Task.FromResult<TUser>(user);
                }
            }

            return Task.FromResult<TUser>(null);
        }

        /// <summary>
        /// Returns list of UserLoginInfo for a given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            List<UserLoginInfo> userLogins = new List<UserLoginInfo>();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            List<UserLoginInfo> logins = UserLoginsTable.FindByUserId(user.Id);
            if (logins != null)
            {
                return Task.FromResult<IList<UserLoginInfo>>(logins);
            }

            return Task.FromResult<IList<UserLoginInfo>>(null);
        }

        /// <summary>
        /// Deletes a login from UserLoginsTable for a given TUser
        /// </summary>
        /// <param name="user">User to have login removed</param>
        /// <param name="login">Login to be removed</param>
        /// <returns></returns>
        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            UserLoginsTable.Delete(user, login);

            return Task.FromResult<Object>(null);
        }

        /// <summary>
        /// Inserts a entry in the UserRoles table
        /// </summary>
        /// <param name="user">User to have role added</param>
        /// <param name="roleName">Name of the role to be added to user</param>
        /// <returns></returns>
        public async Task AddToRoleAsync(TUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Argument cannot be null or empty: roleName.");
            }

            IRole<int> role = await RoleTable.GetRoleByName(roleName);

            if(role != null)
            {
                UserRolesTable.Insert(user, role.Id);
            }
        }

        /// <summary>
        /// Returns the roles for a given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<IList<string>> GetRolesAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var roleNames = await UserRolesTable.FindByUserId(user.Id);

            return roleNames.ToList();
        }

        public async Task<bool> IsInRoleAsync(TUser user, string role)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentNullException("role");
            }

            var roleNames = await UserRolesTable.FindByUserId(user.Id);

            return roleNames.Contains(role);
        }

        public Task RemoveFromRoleAsync(TUser user, string role)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TUser user)
        {
            if (user != null)
            {
                return UserTable.Delete(user);
            }

            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            return UserTable.GetPasswordHash(user.Id);
        }

        public async Task<bool> HasPasswordAsync(TUser user)
        {
            string passwordHash = await UserTable.GetPasswordHash(user.Id);

            return !String.IsNullOrEmpty(passwordHash);
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;

            // TODO: Do we need to update the database?

            return Task.FromResult(0);
        }

        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            user.SecurityStamp = stamp;

            // TODO: Do we need to update the database?

            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(TUser user)
        {
            // TODO: Do we need to query the database?
            return Task.FromResult(user.SecurityStamp);
        }

        public Task SetEmailAsync(TUser user, string email)
        {
            user.Email = email;

            return UserTable.Update(user);
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            // TODO: Do we need to query the database?
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            // TODO: Do we need to query the database?
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            user.EmailConfirmed = confirmed;

            return UserTable.Update(user);
        }


        public Task<TUser> FindByEmailAsync(string email)
        {
            return UserTable.GetUserByEmail(email);
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            user.PhoneNumber = phoneNumber;

            return UserTable.Update(user);
        }

        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            // TODO: Do we need to query the database?
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            // TODO: Do we need to query the database?
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            user.PhoneNumberConfirmed = confirmed;

            return UserTable.Update(user);
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            user.TwoFactorEnabled = enabled;

            return UserTable.Update(user);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            // TODO: Do we need to query the database?
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            // TODO: Do we need to query the database?
            return
                Task.FromResult(user.LockoutEndDateUtc.HasValue
                    ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
                    : new DateTimeOffset());
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;

            return UserTable.Update(user);
        }

        public async Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            user.AccessFailedCount++;

            await UserTable.Update(user);

            return user.AccessFailedCount;
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            user.AccessFailedCount = 0;

            return UserTable.Update(user);
        }

        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            // TODO: Do we need to query the database?
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            // TODO: Do we need to query the database?
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            user.LockoutEnabled = enabled;

            return UserTable.Update(user);
        }
    }
}
