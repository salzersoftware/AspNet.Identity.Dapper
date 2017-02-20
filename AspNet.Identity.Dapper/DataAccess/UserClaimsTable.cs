using AspNet.Identity.Dapper.Connection.Interfaces;
using Dapper;
using System.Security.Claims;

namespace AspNet.Identity.Dapper
{
    /// <summary>
    /// Class that represents the UserClaims table in the Database
    /// </summary>
    public class UserClaimsTable
    {
        private IDbConnectionFactory DbConnectionFactory { get; }

        public UserClaimsTable(IDbConnectionFactory dbConnectionFactory)
        {
            DbConnectionFactory = dbConnectionFactory;
        }

        /// <summary>
        /// Returns a ClaimsIdentity instance given a userId
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public ClaimsIdentity FindByUserId(int memberId)
        {
           ClaimsIdentity claims = new ClaimsIdentity();

            using (var connection = DbConnectionFactory.GetOpenConnection())
            {
                foreach (var c in connection.Query("Select * from MemberClaim where MemberId=@memberId", new { memberId = memberId }))
                {
                    claims.AddClaim(new Claim(c.ClaimType, c.ClaimValue));
                }
            }

           return claims;
        }

        /// <summary>
        /// Deletes all claims from a user given a userId
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public void Delete(int memberId)
        {
            using (var connection = DbConnectionFactory.GetOpenConnection())
            {
                connection.Execute(@"Delete from MemberClaim where UserId = @memberId", new { memberId = memberId });
            }                
        }

        /// <summary>
        /// Inserts a new claim in UserClaims table
        /// </summary>
        /// <param name="claim">User's claim to be added</param>
        /// <param name="userId">User's id</param>
        /// <returns></returns>
        public void Insert(Claim claim, int memberId)
        {
            using (var connection = DbConnectionFactory.GetOpenConnection())
            {
                connection.Execute(@"
                    Insert into MemberClaim (ClaimValue, ClaimType, MemberId) 
                    values (@value, @type, @userId)",
                    new
                    {
                        value = claim.Value,
                        type = claim.Type,
                        userId = memberId
                    }
                );
            }
        }

        /// <summary>
        /// Deletes a claim from a user 
        /// </summary>
        /// <param name="user">The user to have a claim deleted</param>
        /// <param name="claim">A claim to be deleted from user</param>
        /// <returns></returns>
        public void Delete(IdentityMember member, Claim claim)
        {
            using (var connection = DbConnectionFactory.GetOpenConnection())
            {
                connection.Execute(@"
                    Delete from MemberClaim where UserId = @memberId and @ClaimValue = @value and ClaimType = @type",
                    new
                    {
                        memberId = member.Id,
                        ClaimValue = claim.Value,
                        type = claim.Type
                    }
                );
            }
        }
    }
}
