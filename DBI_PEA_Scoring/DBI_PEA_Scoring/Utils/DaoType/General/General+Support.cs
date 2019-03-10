using DBI_PEA_Scoring.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace DBI_PEA_Scoring.Utils.DaoType
{
    partial class General
    {
        public static void GenerateDatabase(string dbSolutionName, string dbAnswerName)
        {
            try
            {
                string queryGenerateAnswerDB = "CREATE DATABASE [" + dbAnswerName + "]\n" +
                                           "GO\n" +
                                           "USE " + "[" + dbAnswerName + "]\n" + Constant.DBScriptList[1];
                ExecuteQuery(queryGenerateAnswerDB, "master");
            }
            catch
            {
            }

            try
            {
                string queryGenerateSolutionDB = "CREATE DATABASE [" + dbSolutionName + "]\n" +
                                             "GO\n" +
                                             "USE " + "[" + dbSolutionName + "]\n" + Constant.DBScriptList[1];
                ExecuteQuery(queryGenerateSolutionDB, "master");
            }
            catch
            {
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

                string dropQuery = "IF EXISTS(SELECT name FROM sys.databases WHERE name = '" + dbName + "')\n" +
                                    "DROP DATABASE " + dbName + "";
                ExecuteQuery(dropQuery , "master");
            }
            catch (Exception)
            {
                // If DB is not exist or some exception here, we let them out.
                Constant.ListDBTemp.Add(dbName);
            }
        }

        /// <summary>
        /// Create 2 database for marking
        /// </summary>
        /// <param name="sourceDbName">DB need to duplicate</param> 
        /// <param name="sqlServerDbFolder">Path to ServerDB: C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\</param>
        /// <param name="newDbName">Name of new DB</param>
        /// 

        //internal static void ClearDatabase()
        //{
        //    foreach (string str in Constant.ListDBTemp)
        //    {
        //        DropDatabase(str);
        //    }
        //    // Clear root database
        //    foreach (Database database in Constant.listDB)
        //    {
        //        DropDatabase(database.SourceDbName);
        //    }
        //    // Clear C:\temp
        //    Directory.Delete(@"C:\Temp", true);
        //}

        public static bool CheckConnection(string dataSource, string userId, string password, string initialCatalog)
        {
            // Save to constant
            Constant.SqlConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = dataSource,
                UserID = userId,
                Password = password,
                InitialCatalog = initialCatalog
            };
            var builder = Constant.SqlConnectionStringBuilder;
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch
                {
                    throw;
                    //return false;
                }
            }
        }

        //public static void GetPathDB(int NoOfDB)
        //{
        //    Constant.listDB = new Database[NoOfDB];
        //    var dataTable = new DataTable();
        //    var query = "use master \nSELECT name, physical_name as path " +
        //                "FROM sys.master_files " +
        //                "where name in (SELECT TOP " + NoOfDB + " Name FROM sys.databases " +
        //                "ORDER BY create_date desc)";
        //    var builder = Constant.SqlConnectionStringBuilder;
        //    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
        //    {
        //        connection.Open();
        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            SqlDataReader reader = command.ExecuteReader();
        //            dataTable.Load(reader);
        //        }
        //    }
        //    int count = 0;
        //    foreach (DataRow row in dataTable.Rows)
        //    {
        //        string name, path;
        //        name = row["name"].ToString();
        //        path = row["path"].ToString();
        //        Constant.listDB[count] = new Database(name, System.IO.Path.GetDirectoryName(path));
        //        count++;
        //    }
        //}

        //public static void SavePathDB()
        //{
        //    var dataTable = new DataTable();
        //    var query = "use master \nSELECT name, physical_name as path " +
        //                "FROM sys.master_files " +
        //                "where name in (SELECT TOP 1 Name FROM sys.databases " +
        //                "ORDER BY create_date desc)";
        //    if (Constant.listDB == null)
        //        Constant.listDB = new Database[0];

        //    // Get the newest database created
        //    var builder = Constant.SqlConnectionStringBuilder;
        //    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
        //    {
        //        connection.Open();
        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            SqlDataReader reader = command.ExecuteReader();
        //            dataTable.Load(reader);
        //        }
        //    }
        //    string name, path;
        //    name = dataTable.Rows[0]["name"].ToString();
        //    path = Path.GetDirectoryName(dataTable.Rows[0]["path"].ToString()) + "\\";

        //    // List database to List
        //    List<Database> temp = new List<Database>();
        //    foreach (Database database in Constant.listDB)
        //    {
        //        temp.Add(database);
        //    }
        //    temp.Add(new Database(path, name));

        //    // Convert List to array
        //    Constant.listDB = temp.ToArray();
        //}
    }
}
