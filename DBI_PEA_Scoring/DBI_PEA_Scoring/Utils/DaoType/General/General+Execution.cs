﻿using System.Data.SqlClient;
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
            query1 = "USE " + db1Name + " \n go \n" + query1;
            query2 = "USE " + db2Name + " \n go \n" + query2;
            ExecuteSingleQuery(query1);
            ExecuteSingleQuery(query2);
        }

        /// <summary>
        /// Execute Single Query    
        /// </summary>
        /// <param name="query">Query to execute</param>
        public static void ExecuteSingleQuery(string query)
        {
            var builder = Common.Constant.SqlConnectionStringBuilder;
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                var server = new Server(new ServerConnection(connection));
                server.ConnectionContext.StatementTimeout = Common.Constant.TimeOutInSecond;
                server.ConnectionContext.ExecuteNonQuery(query);
                server.ConnectionContext.Disconnect();
            }
        }
    }
}