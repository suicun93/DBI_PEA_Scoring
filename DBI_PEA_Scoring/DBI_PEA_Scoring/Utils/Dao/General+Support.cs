using System;
using System.Data.SqlClient;
using DBI_PEA_Scoring.Common;

namespace DBI_PEA_Scoring.Utils.Dao
{
    partial class General
    {
        public static void GenerateDatabase(string dbSolutionName, string dbAnswerName, string dbScript)
        {
            try
            {
                string queryGenerateAnswerDb = "CREATE DATABASE [" + dbAnswerName + "]\n" +
                                           "GO\n" +
                                           "USE " + "[" + dbAnswerName + "]\n" + dbScript;
                ExecuteSingleQuery(queryGenerateAnswerDb, "master");
            }
            catch
            {
                // ignored
            }

            try
            {
                string queryGenerateSolutionDb = "CREATE DATABASE [" + dbSolutionName + "]\n" +
                                             "GO\n" +
                                             "USE " + "[" + dbSolutionName + "]\n" + dbScript;
                ExecuteSingleQuery(queryGenerateSolutionDb, "master");
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Drop a Database
        /// </summary>
        /// <param name="dbName">Database need to drop</param>
        /// <returns>
        /// "message error" if error
        /// "" if done
        /// </returns>
        public static void DropDatabase(string dbName)
        {
            try
            {
                string dropQuery = "USE MASTER IF EXISTS(SELECT name FROM sys.databases WHERE name = '" + dbName + "')\n" +
                                    "USE MASTER DROP DATABASE " + dbName + "";
                ExecuteSingleQuery(dropQuery , "master");
            }
            catch (Exception)
            {
                // If DB is not exist or some exception here, we let them out.
                Constant.ListDBTemp.Add(dbName);
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
                MaxPoolSize = Constant.MaxConnectionPoolSize
            };
            var builder = Constant.SqlConnectionStringBuilder;
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                return true;
            }
        }
    }
}
