using System.Configuration;
using System.Data.SqlClient;
namespace LMS_PROJECT.Models
{
    public class DapperHelper
    {
        public readonly SqlConnection _connection;
        public DapperHelper()
        {
            _connection = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString.ToString());
        }
    }
}