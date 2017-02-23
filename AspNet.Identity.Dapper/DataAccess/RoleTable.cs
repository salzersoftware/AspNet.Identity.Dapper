using AspNet.Identity.Dapper.Connection.Interfaces;
using AspNet.Identity.Dapper.DataAccess.Constants;
using Dapper;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace AspNet.Identity.Dapper
{
    /// <summary>
    /// Class that represents the Role table in the Database
    /// </summary>
    public class RoleTable
    {
        private IDbConnectionFactory DbConnectionFactory { get; }

        public RoleTable(IDbConnectionFactory dbConnectionFactory)
        {
            DbConnectionFactory = dbConnectionFactory;
        }

        public async Task Insert(IdentityRole role)
        {
            using (var connection = await DbConnectionFactory.GetOpenConnectionAsync())
            {
                await connection.ExecuteAsync($@"
                    INSERT INTO [{TableConstants.RoleTable}] (Name) VALUES (@name)",
                    new
                    {
                        name = role.Name
                    }
                );
            }
        }

        public async Task Update(IdentityRole role)
        {
            using (var connection = await DbConnectionFactory.GetOpenConnectionAsync())
            {
                await connection.ExecuteAsync($@"
                    UPDATE [{TableConstants.RoleTable}]
                    SET
                        Name = @name
                    WHERE
                        Id = @id",
                    new
                    {
                        name = role.Name,
                        id = role.Id
                    }
                );
            }
        }

        public async Task Delete(int roleId)
        {
            using (var connection = await DbConnectionFactory.GetOpenConnectionAsync())
            {
                await connection.ExecuteAsync($@"DELETE FROM [{TableConstants.RoleTable}] WHERE Id = @id", new { id = roleId });
            }                
        }

        public string GetRoleName(int roleId)
        {
            using (var connection = DbConnectionFactory.GetOpenConnection())
            {
                return connection.ExecuteScalar<string>($"SELECT Name FROM [{TableConstants.RoleTable}] WHERE Id=@id", new { id = roleId });
            }                
        }

        public async Task<IRole<int>> GetRoleById(int roleId)
        {
            using (var connection = await DbConnectionFactory.GetOpenConnectionAsync())
            {
                IRole<int> role = 
                    await connection.QueryFirstOrDefaultAsync<IRole<int>>($@"
                        SELECT Id, Name
                        FROM [{TableConstants.RoleTable}]
                        WHERE Id=@RoleId",
                        new
                        {
                            RoleId = roleId
                        }
                    );

                return role;
            }
        }

        public async Task<IRole<int>> GetRoleByName(string roleName)
        {
            using (var connection = await DbConnectionFactory.GetOpenConnectionAsync())
            {
                IRole<int> role =
                    await connection.QueryFirstOrDefaultAsync<IRole<int>>($@"
                        SELECT Id, Name
                        FROM [{TableConstants.RoleTable}]
                        WHERE Id=@RoleName",
                        new
                        {
                            RoleName = roleName
                        }
                    );

                return role;
            }
        }
    }
}
