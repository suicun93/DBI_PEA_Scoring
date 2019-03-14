using System;
using System.Data.SqlClient;
using DBI_PEA_Scoring.Common;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace DBI_PEA_Scoring.Utils.Dao
{
    public partial class General
    {
        public static bool PrepareSpCompareDatabase()
        {
            try
            {
                ExecuteSingleQuery(SchemaType.ProcCompareDbsCreate, "master");
            }
            catch
            {
                // ProcCompareDbsCreate has been created
                try
                {
                    ExecuteSingleQuery(SchemaType.ProcCompareDbsAlter, "master");
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

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
        //public static bool ExecuteQuery(string query, string catalog)
        //{
        //    query = "Use " + "[" + catalog + "]\nGO\n" + query + "";
        //    var queryList = query.Split(new[] { "GO", "go", "Go", "gO" }, StringSplitOptions.None);
        //    using (var connection = new SqlConnection(Constant.SqlConnectionStringBuilder.ConnectionString))
        //    {
        //        connection.Open();
        //        try
        //        {
        //            foreach (var q in queryList)
        //            {
        //                using (var command = new SqlCommand(q, connection))
        //                {
        //                    command.CommandTimeout = Constant.TimeOutInSecond;
        //                    Console.WriteLine(command.ExecuteNonQuery());
        //                }
        //            }
        //            return true;
        //        }
        //        finally
        //        {
        //            SqlCommand FixCommand = new SqlCommand("Use master", connection);
        //            FixCommand.ExecuteNonQuery();
        //        }
        //    }
        //}

        /// <summary>
        /// Execute Single Query    
        /// </summary>
        /// <param name="query">Query to execute</param>
        /// <param name="catalog"></param>
        public static bool ExecuteSingleQuery(string query, string catalog)
        {
            query = "Use " + "[" + catalog + "];\nGO\n" + query + "";
            using (var connection = new SqlConnection(Constant.SqlConnectionStringBuilder.ConnectionString))
            {
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

