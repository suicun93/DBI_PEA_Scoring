using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DBI_PEA_Scoring.Common;

namespace DBI_PEA_Scoring.Utils.DaoType
{
    public class General
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
            var builder = Constant.SqlConnectionStringBuilder;
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                string dropQuery = "use master drop database " + dbName + "";
                using (SqlCommand commandDrop = new SqlCommand(dropQuery, connection))
                {
                    connection.Open();
                    commandDrop.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Create 2 database for marking
        /// </summary>
        /// <param name="sourceDbName">DB need to duplicate</param> 
        /// <param name="sqlServerDbFolder">Path to ServerDB: C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\</param>
        /// <param name="newDbName">Name of new DB</param>
        /// 
        public static void DuplicatedDb(string sqlServerDbFolder, string sourceDbName, string newDbName)
        {
            var builder = Constant.SqlConnectionStringBuilder;
            for (int i = 0; i < 2; i++)
            {
                string sql = "declare @sourceDbName nvarchar(50) = '" + sourceDbName + "';\n" +
                             "declare @tmpFolder nvarchar(50) = 'C:\\Temp\\'\n" +
                             "declare @sqlServerDbFolder nvarchar(200) = '" + sqlServerDbFolder + "'\n" +
                             "\n" +
                             "declare @sourceDbFile nvarchar(50);\n" +
                             "declare @sourceDbFileLog nvarchar(50);\n" +
                             "declare @destinationDbName nvarchar(50) = '" + newDbName + "' + '_' + '"
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
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db1Name">DB of student</param>
        /// <param name="db2Name">DB of teacher</param>
        /// <param name="query1">query of student</param>
        /// <param name="query2">query of teacher</param>
        public static void ExecuteQuery(string db1Name, string db2Name, string query1, string query2)
        {
            try
            {
                var builder = Constant.SqlConnectionStringBuilder;
                query1 = "USE " + db1Name + "; \n" + query1;
                query2 = "USE " + db2Name + "; \n" + query2;
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query1, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    using (SqlCommand command = new SqlCommand(query2, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        ////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Compare Table no sort
        /// </summary>
        /// <param name="db1Name"></param>
        /// <param name="db2Name"></param>
        /// <param name="queryTable1"></param>
        /// <param name="queryTable2"></param>
        /// <returns>
        /// "true" if correct
        /// "false" if wrong
        /// message error from sqlserver if error
        /// </returns>
        public static bool CompareTableNoSort(string db1Name, string db2Name, string queryTable1,
            string queryTable2)
        {
            var builder = Constant.SqlConnectionStringBuilder;
            string sql = "USE " + db1Name + "; \n" +
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
                    return command.ExecuteScalar() == null;
                }
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

        /// <summary>
        /// Compare tables with sort
        /// </summary>
        /// <param name="db1Name"></param>
        /// <param name="db2Name"></param>
        /// <param name="queryTable1"></param>
        /// <param name="queryTable2"></param>
        /// <returns>
        /// "true" if correct
        /// "false" if wrong
        /// message error from sqlserver if error
        /// </returns>
        public static bool CompareTableSort(string db1Name, string db2Name, string queryTable1,
            string queryTable2)
        {
            var builder = Constant.SqlConnectionStringBuilder;
            bool resCheckSchema = CompareTableNoSort(db1Name, db2Name, queryTable1, queryTable2);
            if (resCheckSchema)
            {
                string sql1 = "USE " + db1Name + "; \n" +
                             queryTable1;
                string sql2 = "USE " + db2Name + "; \n" +
                             queryTable2;
                // Connect to SQL
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    DataTable dt1 = new DataTable();
                    DataTable dt2 = new DataTable();
                    using (SqlCommand command = new SqlCommand(sql1, connection))
                    {
                        SqlDataReader reader1 = command.ExecuteReader();
                        dt1.Load(reader1);
                    }
                    using (SqlCommand command = new SqlCommand(sql2, connection))
                    {
                        SqlDataReader reader2 = command.ExecuteReader();
                        dt2.Load(reader2);
                    }
                    return !CompareDataTables(dt1, dt2);
                }
            }
            return false;
        }

        /// <summary>
        /// compare datatables same schema
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns>
        /// true = difference
        /// false = same
        /// </returns>
        public static bool CompareDataTables(DataTable dt1, DataTable dt2)
        {
            return dt1.AsEnumerable().Except(dt2.AsEnumerable(), DataRowComparer.Default).Any();
        }
    }
}


