using System.Collections.Generic;
using System.Data.SqlClient;
using DBI_Grading.Model.Teacher;

namespace DBI_Grading.Common
{
    public class Constant
    {
        //public static Database[] listDB = null;
        public static List<string> ListDbTemp = new List<string>();

        //Configure SQL
        public static int TimeOutInSecond;

        public static int MaxThreadPoolSize = 1;
        public static int MaxConnectionPoolSize = 100;

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