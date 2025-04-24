using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System.Data;
using SRM.DDOS.API.Infrastructure.Abstraction;
using Microsoft.Data.SqlClient;

namespace SRM.DDOS.API.Infrastructure
{
    public class SQLDapperContext : IDapperContext, IDisposable
    {
        /// <summary>
        /// The dapper settings
        /// </summary>
        private readonly IConfiguration configuration;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="DapperContext"/> class.
        /// </summary>
        /// <param name="DapperSettings">The dapper settings.</param>
        public SQLDapperContext(IConfiguration config)
        {
            configuration = config;
        }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <returns></returns>
        public IDbConnection Connection
            => GetConnection();

        private IDbConnection GetConnection()
        {
            if (string.IsNullOrEmpty(configuration.GetConnectionString("DefaultConnection")))
            {
                throw new InvalidOperationException(
                    $"Could not found Connection string with name DefaultConnection inside section Connection Strings");
            }

            //return new MySqlConnection(configuration.GetConnectionString("DefaultConnection"));
            return new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Connection is not null)
                    {
                        Connection.Dispose();
                        if (Connection.State == ConnectionState.Open)
                        {
                            Connection.Close();
                        }
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DapperContext()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
