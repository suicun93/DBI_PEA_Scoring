using System.Data.SqlClient;

namespace DBI_PEA_Scoring.Utils.DaoType
{
    public class General
    {
        /// <summary>
        /// Drop a Database
        /// </summary>
        /// <param name="dbName">Database need to drop</param>
        /// <param name="builder">String builder</param>
        /// <returns>
        /// "message error" if error
        /// "" if done
        /// </returns>
        public bool DropDatabase(string dbName, SqlConnectionStringBuilder builder)
        {

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                string dropQuery = "use master drop database " + dbName + "";
                using (SqlCommand commandDrop = new SqlCommand(dropQuery, connection))
                {
                    connection.Open();
                    commandDrop.ExecuteNonQuery();
                    return true;
                }
            }
        }


        /// <summary>
        /// Create 2 database for marking
        /// </summary>
        /// <param name="sourceDbName">DB need to duplicate</param> 
        /// <param name="builder">ConnectionString</param>
        /// <param name="sqlServerDbFolder">Path to ServerDB: C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\</param>
        /// <param name="newDbName"></param>
        /// 
        public bool DuplicatedDb(SqlConnectionStringBuilder builder, string sqlServerDbFolder, string sourceDbName, string newDbName)
        {
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
            return true;
        }
    }
}

