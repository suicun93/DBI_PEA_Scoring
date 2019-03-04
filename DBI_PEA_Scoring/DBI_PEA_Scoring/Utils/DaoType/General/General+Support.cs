using DBI_PEA_Scoring.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DBI_PEA_Scoring.Utils.DaoType
{
    partial class General
    {
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
                var builder = Constant.SqlConnectionStringBuilder;
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    string dropQuery = "use master drop database " + dbName + "";
                    using (SqlCommand commandDrop = new SqlCommand(dropQuery, connection))
                    {
                        connection.Open();
                        commandDrop.CommandTimeout = Constant.TimeOutInSecond;
                        commandDrop.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                // If DB is not exist or some exception here, we let them out.
            }
        }

        /// <summary>
        /// Create 2 database for marking
        /// </summary>
        /// <param name="sourceDbName">DB need to duplicate</param> 
        /// <param name="sqlServerDbFolder">Path to ServerDB: C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\</param>
        /// <param name="newDbName">Name of new DB</param>
        /// 
        public static void DuplicatedDb(string sqlServerDbFolder, string sourceDbName, string newDBName)
        {
            try
            {
                var builder = Constant.SqlConnectionStringBuilder;
                for (int i = 0; i < 2; i++)
                {
                    string sql = "declare @sourceDbName nvarchar(50) = '" + sourceDbName + "';\n" +
                                 "declare @tmpFolder nvarchar(50) = '%temp%'\n" +
                                 "declare @sqlServerDbFolder nvarchar(200) = '" + sqlServerDbFolder + "'\n" +
                                 "\n" +
                                 "declare @sourceDbFile nvarchar(50);\n" +
                                 "declare @sourceDbFileLog nvarchar(50);\n" +
                                 "declare @destinationDbName nvarchar(50) = '" + newDBName + "' + '_' + '"
                                 + ((i % 2 == 0) ? "Student" : "Teacher") + "'\n" +
                                 "declare @backupPath nvarchar(400) = @tmpFolder + @destinationDbName + '.bak'\n" +
                                 "declare @destMdf nvarchar(100) = @sqlServerDbFolder + @destinationDbName + '.mdf'\n" +
                                 "declare @destLdf nvarchar(100) = @sqlServerDbFolder + @destinationDbName + '_log' + '.ldf'\n" +
                                 "\n" +
                                 "SET @sourceDbFile = (SELECT top 1 files.name \n" +
                                 "                    FROM sys.databases dbs \n" +
                                 "                    INNER JOIN sys.master_files files \n" +
                                 "                        ON dbs.database_id = files.database_id \n" +
                                 "                    WHERE dbs.name = @sourceDbName\n" +
                                 "                        AND files.[type] = 0)\n" +
                                 "\n" +
                                 "SET @sourceDbFileLog = (SELECT top 1 files.name \n" +
                                 "                    FROM sys.databases dbs \n" +
                                 "                    INNER JOIN sys.master_files files \n" +
                                 "                        ON dbs.database_id = files.database_id \n" +
                                 "                    WHERE dbs.name = @sourceDbName\n" +
                                 "                        AND files.[type] = 1)\n" +
                                 "\n" +
                                 "BACKUP DATABASE @sourceDbName TO DISK = @backupPath\n" +
                                 "\n" +
                                 "RESTORE DATABASE @destinationDbName FROM DISK = @backupPath\n" +
                                 "WITH REPLACE,\n" +
                                 "   MOVE @sourceDbFile     TO @destMdf,\n" +
                                 "   MOVE @sourceDbFileLog  TO @destLdf";
                    // Connect to SQL
                    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.CommandTimeout = Constant.TimeOutInSecond;
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        internal static void ClearDatabase()
        {
            foreach (Database database in Constant.listDB)
            {
                DropDatabase(database.SourceDbName);
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
                    return false;
                }
            }
        }

        public static void GetPathDB(int NoOfDB)
        {
            Constant.listDB = new Database[NoOfDB];
            var dataTable = new DataTable();
            var query = "use master \nSELECT name, physical_name as path " +
                        "FROM sys.master_files " +
                        "where name in (SELECT TOP " + NoOfDB + " Name FROM sys.databases " +
                        "ORDER BY create_date desc)";
            var builder = Constant.SqlConnectionStringBuilder;
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    dataTable.Load(reader);
                }
            }
            int count = 0;
            foreach (DataRow row in dataTable.Rows)
            {
                string name, path;
                name = row["name"].ToString();
                path = row["path"].ToString();
                Constant.listDB[count] = new Database(name, System.IO.Path.GetDirectoryName(path));
                count++;
            }
        }

        public static void SavePathDB()
        {
            var dataTable = new DataTable();
            var query = "use master \nSELECT name, physical_name as path " +
                        "FROM sys.master_files " +
                        "where name in (SELECT TOP 1 Name FROM sys.databases " +
                        "ORDER BY create_date desc)";
            if (Constant.listDB == null)
                Constant.listDB = new Database[0];

            // Get the newest database created
            var builder = Constant.SqlConnectionStringBuilder;
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    dataTable.Load(reader);
                }
            }
            string name, path;
            name = dataTable.Rows[0]["name"].ToString();
            path = System.IO.Path.GetDirectoryName(dataTable.Rows[0]["path"].ToString()) + "\\";

            // List database to List
            List<Database> temp = new List<Database>();
            foreach (Database database in Constant.listDB)
            {
                temp.Add(database);
            }
            temp.Add(new Database(path, name));

            // Convert List to array
            Constant.listDB = temp.ToArray();
        }
    }
}
