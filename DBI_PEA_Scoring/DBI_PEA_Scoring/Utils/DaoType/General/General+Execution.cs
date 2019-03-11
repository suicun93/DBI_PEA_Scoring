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
        /// Execute Single Query    
        /// </summary>
        /// <param name="query">Query to execute</param>
        public static void ExecuteQuery(string query, string catalog)
        {
            query = "Use " + "[" + catalog + "]\nGO\n" + query + "";
            string[] queryList = query.Split(new string[] { "GO", "go", "Go", "oG" }, StringSplitOptions.None);
            var builder = Common.Constant.SqlConnectionStringBuilder;
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                for (int i = 0; i < queryList.Length; i++)
                {
                    using (SqlCommand command = new SqlCommand(queryList[i], connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

            }
        }


        /// <summary>
        /// Execute Single Query    
        /// </summary>
        /// <param name="query">Query to execute</param>
        //public static void ExecuteSingleQuery(string query, string catalog)
        //{
        //    query = "Use " + "[" + catalog + "];\nGO\n" + query + "";
        //    var builder = Common.Constant.SqlConnectionStringBuilder;
        //    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
        //    {
        //        connection.Open();
        //        var server = new Server(new ServerConnection(connection));
        //        server.ConnectionContext.StatementTimeout = Common.Constant.TimeOutInSecond;
        //        server.ConnectionContext.Connect();
        //        try
        //        {
        //            server.ConnectionContext.ExecuteNonQuery(query);
        //        }
        //        catch (System.Exception)
        //        {
        //            throw;
        //        }
        //        finally
        //        {
        //            server.ConnectionContext.ExecuteNonQuery("Use master");
        //            server.ConnectionContext.Disconnect();
        //        }
        //    }
        //}
    }
}
