using AspNet.Identity.Dapper.Connection.Interfaces;
using AspNet.Identity.Dapper.DataAccess.Constants;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNet.Identity.Dapper
{
    public class UserRolesTable
    {
        private IDbConnectionFactory DbConnectionFactory { get; }

        public UserRolesTable(IDbConnectionFactory dbConnectionFactory)
        {
            DbConnectionFactory = dbConnectionFactory;
        }

        public async Task Insert(IdentityMember user, int roleId)
        {
            using (var connection = await DbConnectionFactory.GetOpenConnectionAsync())
            {
                await connection.ExecuteAsync($@"
                    INSERT INTO {TableConstants.UserRoleTable}
                    (
                        UserId,
                        RoleId
                    )
                    VALUES
                    (
                        @UserId,
                        @RoleId
                    );",
                    new
                    {
                        UserId = user.Id,
                        RoleId = roleId
                    }
                );
            }
        }

        public async Task<IEnumerable<string>> FindByUserId(int userId)
        {
            using (var connection = await DbConnectionFactory.GetOpenConnectionAsync())
            {
                return await connection.QueryAsync<string>($@"
                    SELECT
                        r.Name
                    FROM
                        {TableConstants.RoleTable} r
                        INNER JOIN {TableConstants.UserRoleTable} ur ON ur.RoleId = r.Id
                    WHERE
                        ur.UserId=@UserId",
                    new
                    {
                        UserId = userId
                    }
                );
            }
        }

        public async Task Delete(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
