using AspNet.Identity.Dapper.Connection.Interfaces;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AspNet.Identity.Dapper.Connection
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private string ConnectionString { get; set; }

        public SqlConnectionFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public SqlConnection GetOpenConnection()
        {
            var connection = new SqlConnection(ConnectionString);

            connection.Open();

            return connection;
        }

        public async Task<SqlConnection> GetOpenConnectionAsync()
        {
            var connection = new SqlConnection(ConnectionString);

            await connection.OpenAsync();

            return connection;
        }
    }
}
