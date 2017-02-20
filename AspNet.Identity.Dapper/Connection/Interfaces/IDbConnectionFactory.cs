using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ConsumerApp.Database.Connections.Interfaces
{
    public interface IDbConnectionFactory
    {
        Task<SqlConnection> GetOpenConnectionAsync();
    }
}