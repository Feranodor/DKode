using System.Data;

namespace MyApi
{
    public interface IDapperContext
    {
        IDbConnection CreateConnection();
    }
}
