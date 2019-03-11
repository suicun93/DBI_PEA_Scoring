using System.Data.SqlClient;
//Microsoft.SqlServer.Smo.dll
using Microsoft.SqlServer.Management.Smo;
//Microsoft.SqlServer.ConnectionInfo.dll    
using Microsoft.SqlServer.Management.Common;
using System;
using System.Threading;

namespace DBI_PEA_Scoring.Utils.DaoType
{
    public partial class General
    {
        /// <summary>
        /// execute a query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="catalog"></param>
        /// <returns>
        /// "" if success
        /// "message error" if error
        /// "false" if not success
        /// </returns>
        public static bool ExecuteQuery(string query, string catalog)
        {
            query = "Use " + "[" + catalog + "]\nGO\n" + query + "";
            string[] queryList = query.Split(new string[] { "GO", "go", "Go", "oG" }, StringSplitOptions.None);
            var builder = Common.Constant.SqlConnectionStringBuilder;
            try
            {
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    for (int i = 0; i < queryList.Length; i++)
                    {
                        using (SqlCommand command = new SqlCommand(queryList[i], connection))
                        {
                            Console.WriteLine(command.ExecuteNonQuery());
                        }
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Execute Single Query    
        /// </summary>
        /// <param name="query">Query to execute</param>
        public static bool ExecuteSingleQuery(string query, string catalog)
        {
            query = "Use " + "[" + catalog + "];\nGO\n" + query + "";
            var builder = Common.Constant.SqlConnectionStringBuilder;
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                var server = new Server(new ServerConnection(connection));
                server.ConnectionContext.StatementTimeout = Common.Constant.TimeOutInSecond;
                server.ConnectionContext.Connect();
                try
                {
                    server.ConnectionContext.ExecuteNonQuery(query);
                    return true;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    server.ConnectionContext.ExecuteNonQuery("Use master");
                    server.ConnectionContext.Disconnect();
                }
            }
        }
    }
}

