using AspNet.Identity.Dapper.Connection.Interfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet.Identity.Dapper.Store
{
    /// <summary>
    /// Class that implements the key ASP.NET Identity role store iterfaces
    /// </summary>
    public class RoleStore<TRole> : IQueryableRoleStore<TRole,int>
        where TRole : IdentityRole
    {
        private IDbConnectionFactory DbConnectionFactory { get; }
        private RoleTable RoleTable { get; }
        public IQueryable<TRole> Roles { get { throw new NotImplementedException(); } }

        public RoleStore(IDbConnectionFactory dbConnectionFactory)
        {
            DbConnectionFactory = dbConnectionFactory;
            RoleTable = new RoleTable(dbConnectionFactory);
        }

        public Task CreateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return RoleTable.Insert(role);
        }

        public Task UpdateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return RoleTable.Update(role);
        }

        public Task DeleteAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return RoleTable.Delete(role.Id);
        }

        public async Task<TRole> FindByIdAsync(int roleId)
        {
            var role = await RoleTable.GetRoleById(roleId) as TRole;

            return role;
        }

        public Task<TRole> FindByNameAsync(string roleName)
        {
            TRole result = RoleTable.GetRoleByName(roleName) as TRole;

            return Task.FromResult<TRole>(result);
        }

        public void Dispose()
        {
            // Nothing to dispose.
        }

    }
}
