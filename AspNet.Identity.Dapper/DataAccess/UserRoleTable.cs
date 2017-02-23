using AspNet.Identity.Dapper.Connection.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet.Identity.Dapper
{
    /// <summary>
    /// Class that represents the UserRoles table in the Database
    /// </summary>
    public class UserRolesTable
    {
        private const string UserRolesTableName = "User_Role";

        private IDbConnectionFactory DbConnectionFactory { get; }

        public UserRolesTable(IDbConnectionFactory dbConnectionFactory)
        {
            DbConnectionFactory = dbConnectionFactory;
        }

        public async Task Insert(IdentityMember member, int roleId)
        {
            using (var connection = await DbConnectionFactory.GetOpenConnectionAsync())
            {
                await connection.ExecuteAsync($@"
                    Insert into {UserRolesTableName} (UserId, RoleId) values (@userId, @roleId",
                    new { userId = member.Id, roleId = roleId }
                );
            }
        }

        public async Task<IEnumerable<string>> FindByUserId(int userId)
        {
            using (var connection = await DbConnectionFactory.GetOpenConnectionAsync())
            {
                return await connection.QueryAsync<string>($@"
                    Select r.Name
                    from {UserRolesTableName} ur
                    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
                    where ur.UserId=@UserId",
                    new { UserId = userId }
                );
            }
        }

        public async Task Delete(int userId)
        {
            using (var connection = await DbConnectionFactory.GetOpenConnectionAsync())
            {
                connection.ExecuteAsync(@"Delete from MemberRole where Id = @MemberId", new { MemberId = userId });
            }
        }
    }
}
