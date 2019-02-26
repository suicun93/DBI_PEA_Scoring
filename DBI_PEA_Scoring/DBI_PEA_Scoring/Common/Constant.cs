using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DBI_PEA_Scoring.Common
{
    public class Constant
    {
        public static DB[] listDB = {new DB(@"C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\", "sample") };
        public static SqlConnectionStringBuilder SqlConnectionStringBuilder = null;
    }
    public class DB
    {
        public DB(string sqlServerDbFolder, string sourceDbName)
        {
            SqlServerDbFolder = sqlServerDbFolder;
            SourceDbName = sourceDbName;
        }
        public string SqlServerDbFolder { get; set; }
        public string SourceDbName { get; set; }

    }
}
