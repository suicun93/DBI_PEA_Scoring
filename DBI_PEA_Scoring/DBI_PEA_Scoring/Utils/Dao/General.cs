using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;
using DBI_PEA_Scoring.Common;
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
                string query = string.Concat("USE ", databaseName,
                    "\nSELECT COUNT(*) from information_schema.tables\r\nWHERE table_type = \'base table\'");
                //Prepare connection
                var builder = Constant.SqlConnectionStringBuilder;
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        return (int)command.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Compare error at count tables: " + e.Message);
            }

        }

        public static DataSet GetDataSetFromReader(string query)
        {
            try
            {
                var builder = Constant.SqlConnectionStringBuilder;
                DataSet dts = new DataSet();
                // Connect to SQL
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
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
        /// 
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
                catch
                {
                    return false;
                }
            }
            return true;
        }

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

        public static List<TestCase> GetTestCasesPoint(string input)
        {
            Match matchResult = Regex.Match(input, @"(/\*(.|[\r\n])*?\*/)|(--(.*|[\r\n]))", RegexOptions.Singleline);

            List<TestCase> tcpList = new List<TestCase>();
            int count = 0;
            TestCase tcp = new TestCase();
            while (matchResult.Success)
            {
                string matchFormatted = matchResult.Value.Split('*')[1];
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
            return tcpList;
        }


    }

    public class TestCase
    {
        public double Point { get; set; }
        public string Description { get; set; }
    }
}

