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

        /// <summary>
        /// Get DataSet
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
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
        /// Get DataSet
        /// </summary>
        /// <param name="query"></param>
        /// <returns>datatable[0] as Schema of Table
        /// datatable[1] as Data of Table
        /// </returns>
        public static DataTable[] GetDataTableFromReader(string query)
        {
            DataTable[] dataTables = new DataTable[2];
            var builder = Constant.SqlConnectionStringBuilder;
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                using (var sqlCommandAnswer = new SqlCommand(query, connection))
                {
                    sqlCommandAnswer.CommandTimeout = Constant.TimeOutInSecond;
                    var sqlReaderAnswer = sqlCommandAnswer.ExecuteReader();
                    dataTables[0] = sqlReaderAnswer.GetSchemaTable();
                    dataTables[1].Load(sqlReaderAnswer);
                }
                return dataTables;
            }
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

        public static void GenerateDatabase(string dbSolutionName, string dbAnswerName, string dbScript)
        {
            try
            {
                if (string.IsNullOrEmpty(dbScript.Trim()))
                    throw new Exception("DbScript for grading is empty!!!\n");

                var queryGenerateAnswerDb = "CREATE DATABASE [" + dbAnswerName + "]\n" +
                                            "GO\n" +
                                            "USE " + "[" + dbAnswerName + "]\n" + dbScript;
                ExecuteSingleQuery(queryGenerateAnswerDb, "master");

                var queryGenerateSolutionDb = "CREATE DATABASE [" + dbSolutionName + "]\n" +
                                              "GO\n" +
                                              "USE " + "[" + dbSolutionName + "]\n" + dbScript;
                ExecuteSingleQuery(queryGenerateSolutionDb, "master");
            }
            catch (Exception e)
            {
                throw new Exception("Generate databases error: " + e.Message + "\n");
            }
        }

        /// <summary>
        ///     Drop a Database
        /// </summary>
        /// <param name="dbName">Database need to drop</param>
        /// <returns>
        ///     "message error" if error
        ///     "" if done
        /// </returns>
        public static void DropDatabase(string dbName)
        {
            try
            {
                var dropQuery = "DROP DATABASE [" + dbName + "]";
                ExecuteSingleQuery(dropQuery, "master");
            }
            catch (Exception)
            {
                // If DB is not exist or some exception here, we let them out.
                Constant.ListDbTemp.Add(dbName);
            }
        }

        public static bool CheckConnection(string dataSource, string userId, string password, string initialCatalog)
        {
            // Save to constant
            Constant.SqlConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = dataSource,
                UserID = userId,
                Password = password,
                InitialCatalog = initialCatalog,
                MinPoolSize = Constant.MaxConnectionPoolSize,
                MaxPoolSize = Constant.MaxConnectionPoolSize,
                ConnectTimeout = Constant.TimeOutInSecond
            };
            var builder = Constant.SqlConnectionStringBuilder;
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                // Save to app config when login successfully
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["serverName"].Value = dataSource;
                config.AppSettings.Settings["username"].Value = userId;
                config.AppSettings.Settings["password"].Value = password;
                config.AppSettings.Settings["initialCatalog"].Value = initialCatalog;
                config.Save(ConfigurationSaveMode.Full, true);
                ConfigurationManager.RefreshSection("appSettings");
                return true;
            }
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