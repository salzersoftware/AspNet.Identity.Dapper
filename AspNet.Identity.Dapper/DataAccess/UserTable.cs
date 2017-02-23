using AspNet.Identity.Dapper.Connection.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System;
using AspNet.Identity.Dapper.DataAccess.Constants;

namespace AspNet.Identity.Dapper
{
    /// <summary>
    /// Class that represents the Users table in the Database
    /// </summary>
    public class UserTable<TUser>
        where TUser : IdentityMember
    {
        private IDbConnectionFactory DbConnectionFactory { get; }

        public UserTable(IDbConnectionFactory dbConnectionFactory)
        {
            DbConnectionFactory = dbConnectionFactory;
        }

        public async Task<TUser> GetUserById(int userId)
        {
            using (var connection = await DbConnectionFactory.GetOpenConnectionAsync())
            {
                TUser user =
                    await connection.QueryFirstOrDefaultAsync<TUser>($@"
                        SELECT *
                        FROM [{TableConstants.UserTable}]
                        WHERE Id=@UserId",
                        new
                        {
                            UserId = userId
                        }
                    );

                return user;
            }
        }

        public async Task<TUser> GetUserByName(string userName)
        {
            using (SqlConnection connection = await DbConnectionFactory.GetOpenConnectionAsync())
            {
                TUser user = 
                    await connection.QueryFirstOrDefaultAsync<TUser>($@"
                        SELECT *
                        FROM [{TableConstants.UserTable}]
                        WHERE UserName=@UserName",
                        new
                        {
                            UserName = userName
                        }
                    );

                return user;
            }
        }

        public async Task<TUser> GetUserByEmail(string email)
        {
            using (SqlConnection connection = await DbConnectionFactory.GetOpenConnectionAsync())
            {
                TUser user =
                    await connection.QueryFirstOrDefaultAsync<TUser>($@"
                        SELECT *
                        FROM [{TableConstants.UserTable}]
                        WHERE Email=@Email",
                        new
                        {
                            Email = email
                        }
                    );

                return user;
            }
        }

        public async Task<string> GetPasswordHash(int memberId)
        {
            using (var connection = await DbConnectionFactory.GetOpenConnectionAsync())
            {
                string passwordHash =
                    await connection.ExecuteScalarAsync<string>($@"
                        SELECT PasswordHash
                        FROM [{TableConstants.UserTable}]
                        WHERE Id = @MemberId",
                        new
                        {
                            MemberId = memberId
                        }
                    );

                return passwordHash;
            }
        }

        public async Task SetPasswordHash(int userId, string passwordHash)
        {
            using (var connection = await DbConnectionFactory.GetOpenConnectionAsync())
            {
                await connection.ExecuteAsync($@"
                    UPDATE
                        [{TableConstants.UserTable}]
                    SET
                        PasswordHash = @PasswordHash
                    WHERE
                        Id = @Id",
                    new
                    {
                        PasswordHash = passwordHash,
                        Id = userId
                    }
                );
            }
        }

        public async Task<int> Insert(TUser member)
        {
            using (var connection = await DbConnectionFactory.GetOpenConnectionAsync())
            {
                int autoIncrementId =
                    await connection.ExecuteScalarAsync<int>($@"
                        INSERT INTO [{TableConstants.UserTable}]
                        (
                            UserName,
                            PasswordHash,
                            SecurityStamp,
                            Email,
                            EmailConfirmed,
                            PhoneNumber,
                            PhoneNumberConfirmed,
                            AccessFailedCount,
                            LockoutEnabled,
                            LockoutEndDateUtc,
                            TwoFactorEnabled
                        )
                        VALUES
                        (
                            @name,
                            @pwdHash,
                            @SecStamp,
                            @email,
                            @emailconfirmed,
                            @phonenumber,
                            @phonenumberconfirmed,
                            @accesscount,
                            @lockoutenabled,
                            @lockoutenddate,
                            @twofactorenabled
                        )

                        SELECT Cast(SCOPE_IDENTITY() as int);",
                        new
                        {
                            name = member.UserName,
                            pwdHash = member.PasswordHash,
                            SecStamp = member.SecurityStamp,
                            email = member.Email,
                            emailconfirmed = member.EmailConfirmed,
                            phonenumber = member.PhoneNumber,
                            phonenumberconfirmed = member.PhoneNumberConfirmed,
                            accesscount = member.AccessFailedCount,
                            lockoutenabled = member.LockoutEnabled,
                            lockoutenddate = member.LockoutEndDateUtc,
                            twofactorenabled = member.TwoFactorEnabled
                        }
                    );

                return autoIncrementId;
            }
        }

        public async Task Delete(TUser user)
        {
            using (var connection = await DbConnectionFactory.GetOpenConnectionAsync())
            {
                await connection.ExecuteAsync($@"
                    DELETE FROM [{TableConstants.UserTable}]
                    WHERE Id = @UserId",
                    new
                    {
                        UserId = user.Id
                    }
                );
            }
        }

        public async Task Update(TUser member)
        {
            using (var connection = await DbConnectionFactory.GetOpenConnectionAsync())
            {
                await connection.ExecuteAsync($@"
                    UPDATE
                        [{TableConstants.UserTable}]
                    SET
                        UserName=@userName,
                        PasswordHash=@pswHash,
                        SecurityStamp = @secStamp, 
                        Email=@email,
                        EmailConfirmed=@emailconfirmed,
                        PhoneNumber=@phonenumber,
                        PhoneNumberConfirmed=@phonenumberconfirmed,
                        AccessFailedCount=@accesscount,
                        LockoutEnabled=@lockoutenabled,
                        LockoutEndDateUtc=@lockoutenddate,
                        TwoFactorEnabled=@twofactorenabled  
                    WHERE Id = @memberId",
                    new
                    {
                        userName = member.UserName,
                        pswHash = member.PasswordHash,
                        secStamp = member.SecurityStamp,
                        memberId = member.Id,
                        email = member.Email,
                        emailconfirmed = member.EmailConfirmed,
                        phonenumber = member.PhoneNumber,
                        phonenumberconfirmed = member.PhoneNumberConfirmed,
                        accesscount = member.AccessFailedCount,
                        lockoutenabled = member.LockoutEnabled,
                        lockoutenddate = member.LockoutEndDateUtc,
                        twofactorenabled = member.TwoFactorEnabled
                    }
                );
            }
        }
    }
}
