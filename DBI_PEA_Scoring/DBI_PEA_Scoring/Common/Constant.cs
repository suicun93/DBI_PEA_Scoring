using System.Collections.Generic;
using System.Data.SqlClient;

namespace DBI_PEA_Scoring.Common
{
    public class Constant
    {
        //public static Database[] listDB = null;
        public static List<string> ListDBTemp = new List<string>();
        public static int NumberOfQuestion = 10;
        public static List<string> DBScriptList = new List<string>();

        // Configure SQL
        public static int TimeOutInSecond = 5;
        public static int MaxThreadPoolSize = 1;
        public static int MaxConnectionPoolSize = 1;
        public static double minusPoint = 0.25;
        // This will be configured in General.cs when user check connection to sql.
        public static SqlConnectionStringBuilder SqlConnectionStringBuilder = null;

    }
    public class Database
    {
        public Database(string sqlServerDbFolder, string sourceDbName)
        {
            SqlServerDbFolder = sqlServerDbFolder;
            SourceDbName = sourceDbName;
        }
        public Database(string sqlServerDbFolder, string source, string sourceDbName)
        {
            SqlServerDbFolder = sqlServerDbFolder;
            Source = source;
            SourceDbName = sourceDbName;
        }

        public string SqlServerDbFolder { get; set; }
        public string SourceDbName { get; set; }
        public string Source { get; set; }

    }
}
