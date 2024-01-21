using System.Data;
using System.Data.SqlClient;

namespace MyApi
{
    public class DapperContext : IDapperContext
    {
        private readonly IConfiguration _iConfiguration;
        private readonly string _connString;
        public DapperContext(IConfiguration iConfiguration)
        {
            _iConfiguration = iConfiguration;
            _connString = _iConfiguration.GetConnectionString("Default");
        }
        public IDbConnection CreateConnection() => new SqlConnection(_connString);
    }
}
