using System.Collections.Generic;
using System.Data.SqlClient;
using DBI_PEA_Scoring.Model.Teacher;

namespace DBI_PEA_Scoring.Common
{
    public class Constant
    {
        //public static Database[] listDB = null;
        public static List<string> ListDBTemp = new List<string>();
        public static int NumberOfQuestion = 10;

        //Configure SQL
        public static int TimeOutInSecond = 10;
        public static int MaxThreadPoolSize = 1;
        public static int MaxConnectionPoolSize = 1;
        public static double minusPoint = 0.1;
        // This will be configured in General.cs when user check connection to sql.
        public static SqlConnectionStringBuilder SqlConnectionStringBuilder = null;

        //PaperSet
        public static PaperSet PaperSet;

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
