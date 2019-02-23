using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DBI_PEA_Scoring.Utils
{
    internal class DaoUtils
    {
        /// <summary>
        /// Compare Schema of Database
        /// </summary>
        /// <param name="db1Name"></param>
        /// <param name="db2Name"></param>
        /// <param name="queryTable1"></param>
        /// <param name="queryTable2"></param>
        /// <param name="dataSource"></param>//name host (localhost)
        /// <param name="userId"></param>//user server
        /// <param name="password"></param>//pass server
        /// <returns>
        /// "true" if correct
        /// "false" if wrong
        /// message error from sqlserver if error
        /// </returns>
        private static string CompareTableNoSort(string db1Name, string db2Name, string queryTable1,
            string queryTable2, string dataSource, string userId, string password)
        {
            try
            {
                // Build connection string
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
                {
                    DataSource = dataSource,
                    UserID = userId,
                    Password = password,
                    InitialCatalog = "master"
                };
                String sql =
                             "USE " + db1Name + "; \n" +
                             "WITH T1 as (" + queryTable1 + ") \n" +
                             "select * INTO #TABLE1 from T1;\n" +
                             "USE " + db2Name + ";\n" +
                             "WITH T2 as (" + queryTable2 + ") \n" +
                             "select * INTO #TABLE2 from T2;\n" +
                             "SELECT * FROM #TABLE1\n" +
                             "EXCEPT \n" +
                             "SELECT * FROM #TABLE2\n" +
                             "\n" +
                             "drop table #TABLE1\n" +
                             "drop table #TABLE2";
                // Connect to SQL
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        if (command.ExecuteScalar() == null)
                        {
                            return "true";
                        }
                        return "false";
                    }
                }
            }
            catch (SqlException e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// Compare Schema of 2 databases
        /// </summary>
        /// <param name="db1Name"></param>
        /// <param name="db2Name"></param>
        /// <param name="dataSource"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns>
        /// "true" if correct
        /// "false" if wrong
        /// message error from sqlserver if error
        /// </returns>
        private static string CompareSchema(string db1Name, string db2Name, string dataSource, string userId, string password)
        {
            try
            {
                // Build connection string
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
                {
                    DataSource = dataSource,
                    UserID = userId,
                    Password = password,
                    InitialCatalog = "master"
                };
                String sql = "USE " + db1Name + "\n" +
                             "SELECT name, system_type_id, user_type_id,max_length, precision,scale, is_nullable, is_identity INTO #DBSchema FROM sys.columns\n" +
                             "WHERE object_id = OBJECT_ID(N'dbo.FirstComTable')\n" +
                             "USE " + db2Name + "\n" +
                             "SELECT name, system_type_id, user_type_id,max_length, precision,scale, is_nullable, is_identity INTO #DB2Schema FROM sys.columns\n" +
                             "WHERE object_id = OBJECT_ID(N'dbo.FirstComTable ');\n" +
                             "SELECT * FROM #DBSchema\n" +
                             "EXCEPT \n" +
                             "SELECT * FROM #DB2Schema\n" +
                             "drop table #DB2Schema\n" +
                             "drop table #DBSchema";
                Console.WriteLine(sql);
                // Connect to SQL
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        if (command.ExecuteScalar() == null)
                        {
                            return "true";
                        }
                        return "false";
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
                return "false";
            }
        }



        /// <summary>
        /// Duplicate a number of a database
        /// </summary>
        /// <param name="dataSource"></param> Name of server
        /// <param name="userId"></param> sa
        /// <param name="password"></param> 123
        /// <param name="initialCatalog"></param> master
        /// <param name="sourceDbName"></param> DB need to duplicate
        /// <param name="sqlServerDbFolder"></param> Path to ServerDB: C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\
        /// <param name="num"></param> times
        public static bool DuplicatedDb(string dataSource, string userId, string password,
            string initialCatalog, string sourceDbName, string sqlServerDbFolder, int num)
        {
            try
            {
                // Build connection string
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
                {
                    DataSource = dataSource,
                    UserID = userId,
                    Password = password,
                    InitialCatalog = "master"
                };
                for (int i = 0; i < num; i++)
                {
                    string sql = "declare @sourceDbName nvarchar(50) = '" + sourceDbName + "';\n" +
                                 "declare @tmpFolder nvarchar(50) = 'C:\\Temp\\'\n" +
                                 "declare @sqlServerDbFolder nvarchar(200) = '" + sqlServerDbFolder + "'\n" +
                                 "\n" +
                                 "declare @sourceDbFile nvarchar(50);\n" +
                                 "declare @sourceDbFileLog nvarchar(50);\n" +
                                 "declare @destinationDbName nvarchar(50) = @sourceDbName + '_' + '" + i + "'\n" +
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
                            command.ExecuteNonQuery();
                        }
                    }
                }
                return true;
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }
    }
}
