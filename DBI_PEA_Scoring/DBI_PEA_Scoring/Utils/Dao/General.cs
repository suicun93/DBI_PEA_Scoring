using System;
using System.Data.SqlClient;
using DBI_PEA_Scoring.Common;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace DBI_PEA_Scoring.Utils.Dao
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
            var queryList = query.Split(new[] { "GO", "go", "Go", "gO" }, StringSplitOptions.None);
            var builder = Constant.SqlConnectionStringBuilder;
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                foreach (var q in queryList)
                {
                    using (var command = new SqlCommand(q, connection))
                    {
                        command.CommandTimeout = 1;
                        Console.WriteLine(command.ExecuteNonQuery());
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Execute Single Query    
        /// </summary>
        /// <param name="query">Query to execute</param>
        /// <param name="catalog"></param>
        public static bool ExecuteSingleQuery(string query, string catalog)
        {
            query = "Use " + "[" + catalog + "];\nGO\n" + query + "";
            var builder = Constant.SqlConnectionStringBuilder;
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                var server = new Server(new ServerConnection(connection));
                server.ConnectionContext.StatementTimeout = Constant.TimeOutInSecond;
                server.ConnectionContext.Connect();
                try
                {
                    server.ConnectionContext.ExecuteNonQuery(query);
                    return true;
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

