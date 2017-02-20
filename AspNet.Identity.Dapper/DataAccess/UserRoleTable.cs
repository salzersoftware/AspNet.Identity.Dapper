using AspNet.Identity.Dapper.Connection.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace AspNet.Identity.Dapper
{
    /// <summary>
    /// Class that represents the UserRoles table in the Database
    /// </summary>
    public class UserRolesTable
    {
        private IDbConnectionFactory DbConnectionFactory { get; }

        public UserRolesTable(IDbConnectionFactory dbConnectionFactory)
        {
            DbConnectionFactory = dbConnectionFactory;
        }

        /// <summary>
        /// Returns a list of user's roles
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public List<string> FindByUserId(int memberId)
        {
            using (var connection = DbConnectionFactory.GetOpenConnection())
            {
                return connection.Query<string>("Select Role.Name from MemberRole, Role where MemberRole.MemberId=@MemberId and MemberRole.RoleId = Role.Id", new { MemberId = memberId })
                .ToList();
            }
        }

        /// <summary>
        /// Deletes all roles from a user in the UserRoles table
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public void Delete(int memberId)
        {
            using (var connection = DbConnectionFactory.GetOpenConnection())
            {
                connection.Execute(@"Delete from MemberRole where Id = @MemberId", new { MemberId = memberId });
            }
        }

        /// <summary>
        /// Inserts a new role for a user in the UserRoles table
        /// </summary>
        /// <param name="user">The User</param>
        /// <param name="roleId">The Role's id</param>
        /// <returns></returns>
        public void Insert(IdentityMember member, int roleId)
        {
            using (var connection = DbConnectionFactory.GetOpenConnection())
            {
                connection.Execute(@"Insert into AspNetUserRoles (UserId, RoleId) values (@userId, @roleId",
                new { userId = member.Id, roleId = roleId });
            }
        }
    }
}
