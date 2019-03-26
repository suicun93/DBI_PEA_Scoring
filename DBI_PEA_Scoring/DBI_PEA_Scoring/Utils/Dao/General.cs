using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;
using DBI_PEA_Scoring.Common;
using DBI_PEA_Scoring.Model;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace DBI_PEA_Scoring.Utils.Dao
{
    public partial class General
    {
        public static int GetNumberOfTablesInDatabase(string databaseName)
        {
            try
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
                        return (int) command.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static DataSet GetDataSetFromReader(string query)
        {
            try
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
            catch (Exception e)
            {
                throw e;
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
                try
                {
                    ExecuteSingleQuery("CREATE " + SchemaType.ProcCompareDbs, "master");
                }
                catch (Exception e)
                {
                    throw e;
                }
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