using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AspNet.Identity.Dapper.Connection.Interfaces
{
    public interface IDbConnectionFactory
    {
        SqlConnection GetOpenConnection();
        Task<SqlConnection> GetOpenConnectionAsync();
    }
}