using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.Dapper.DataAccess.Constants
{
    public class TableConstants
    {
        public const string UserTable = "AuthUser";
        public const string RoleTable = "AuthRole";
        public const string UserLoginTable = "AuthUserLogin";
        public const string UserRoleTable = "AuthUser_AuthRole";
        public const string UserClaimTable = "AuthUserClaim";
    }
}
