using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;
using DBI_Grading.Common;
using DBI_Grading.Model.Teacher;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace DBI_Grading.Utils.Dao
{
    public partial class General
    {
        public static int GetNumberOfTablesInDatabase(string databaseName)
        {
            var query = string.Concat("USE [", databaseName,
                "]\nSELECT COUNT(*) from information_schema.tables\r\nWHERE table_type = \'base table\'");
            //Prepare connection
            var builder = Constant.SqlConnectionStringBuilder;
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public static DataSet GetDataSetFromReader(string query)
        {
            var builder = Constant.SqlConnectionStringBuilder;
            var dts = new DataSet();
            // Connect to SQL
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                //1. Check number of tables
                using (var sqlDataAdapter = new SqlDataAdapter(query, connection))
                {
                    sqlDataAdapter.Fill(dts);
                }
            }
            return dts;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static bool PrepareSpCompareDatabase()
        {
            try
            {
                ExecuteSingleQuery("ALTER " + SchemaType.ProcCompareDbs, "master");
            }
            catch
            {
                // ProcCompareDbsCreate has been created
                ExecuteSingleQuery("CREATE " + SchemaType.ProcCompareDbs, "master");
            }
            return true;
        }

        /// <summary>
        ///     Execute Single Query
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

        public static void KillAllSessionSql()
        {
            try
            {
                string queryKillNow = "use master ";
                var builder = new SqlConnectionStringBuilder()
                {
                    ConnectTimeout = Constant.TimeOutInSecond,
                    DataSource = ConfigurationManager.AppSettings["serverName"],
                    IntegratedSecurity = true,
                    InitialCatalog = ConfigurationManager.AppSettings["initialCatalog"]
                };
                using (SqlConnection conn = new SqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    string queryGetSession =
                        "use master SELECT conn.session_id FROM sys.dm_exec_sessions AS sess JOIN sys.dm_exec_connections AS conn " +
                        "ON sess.session_id = conn.session_id where program_name = '.Net SqlClient Data Provider' order by session_id asc";
                    int countSession = 0;
                    using (var command = new SqlCommand(queryGetSession, conn))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                queryKillNow += " KILL " + id + " ";
                                countSession++;
                            }
                        }
                    }
                    using (var command = new SqlCommand(queryKillNow, conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static List<TestCase> GetTestCasesPoint(Candidate candidate)
        {
            var matchResult = Regex.Match(candidate.TestQuery, @"(/\*(.|[\r\n])*?\*/)|(--(.*|[\r\n]))",
                RegexOptions.Singleline);

            var tcpList = new List<TestCase>();
            var count = 0;
            var tcp = new TestCase();
            while (matchResult.Success)
            {
                var matchFormatted = matchResult.Value.Split('*')[1];
                if (count++ % 2 == 0)
                {
                    tcp.Point = double.Parse(matchFormatted, CultureInfo.InvariantCulture);
                }
                else
                {
                    tcp.Description = matchFormatted;
                    tcpList.Add(tcp);
                    tcp = new TestCase();
                }
                matchResult = matchResult.NextMatch();
            }
            if (tcpList.Count == 0)
                tcpList.Add(new TestCase
                {
                    Description = "",
                    Point = candidate.Point
                });
            return tcpList;
        }
    }

    public class TestCase
    {
        public double Point { get; set; }
        public string Description { get; set; }
    }
}