using ConsumerApp.Database.Connections.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerApp.Database.Connections
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private string ConnectionString { get; set; }

        public SqlConnectionFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public async Task<SqlConnection> GetOpenConnectionAsync()
        {
            var connection = new SqlConnection(ConnectionString);

            await connection.OpenAsync();

            return connection;
        }
    }
}
