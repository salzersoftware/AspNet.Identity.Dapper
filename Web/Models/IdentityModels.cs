using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using AspNet.Identity.Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System;


namespace Web.Models
{
    // You can add profile data for the AppMember by adding more properties to your AppMember class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class AppMember : IdentityMember
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AppMember, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom AppMember claims here
            return userIdentity;
        }
    }
}