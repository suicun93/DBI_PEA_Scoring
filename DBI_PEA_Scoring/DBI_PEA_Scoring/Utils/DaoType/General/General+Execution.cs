using System.Data.SqlClient;
//Microsoft.SqlServer.Smo.dll
using Microsoft.SqlServer.Management.Smo;
//Microsoft.SqlServer.ConnectionInfo.dll
using Microsoft.SqlServer.Management.Common;

namespace DBI_PEA_Scoring.Utils.DaoType
{
    public partial class General
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db1Name">DB of student</param>
        /// <param name="db2Name">DB of teacher</param>
        /// <param name="query1">query of student</param>
        /// <param name="query2">query of teacher</param>
        public static void ExecuteQuery(string db1Name, string db2Name, string query1, string query2)
        {
            try
            {
                var builder = DBI_PEA_Scoring.Common.Constant.SqlConnectionStringBuilder;
                query1 = "USE " + db1Name + "; \n" + query1;
                query2 = "USE " + db2Name + "; \n" + query2;
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    var server = new Server(new ServerConnection(connection));
                    server.ConnectionContext.ExecuteNonQuery(query1);
                    server.ConnectionContext.Disconnect();
                }
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    var server = new Server(new ServerConnection(connection));
                    server.ConnectionContext.ExecuteNonQuery(query2);
                    server.ConnectionContext.Disconnect();
                }
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Execute Single Query    
        /// </summary>
        /// <param name="query">Query to execute</param>
        public static void ExecuteSingleQuery(string query)
        {
            var builder = DBI_PEA_Scoring.Common.Constant.SqlConnectionStringBuilder;
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                var server = new Server(new ServerConnection(connection));
                server.ConnectionContext.ExecuteNonQuery(query);
                server.ConnectionContext.Disconnect();
            }
        }
    }
}
