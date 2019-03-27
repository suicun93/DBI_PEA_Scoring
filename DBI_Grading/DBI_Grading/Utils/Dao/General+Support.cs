using System;
using System.Configuration;
using System.Data.SqlClient;
using DBI_PEA_Grading.Common;

namespace DBI_PEA_Grading.Utils.Dao
{
    partial class General
    {
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
    }
}