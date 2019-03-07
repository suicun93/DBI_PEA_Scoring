using System.Collections.Generic;
using System.Data.SqlClient;

namespace DBI_PEA_Scoring.Common
{
    public class Constant
    {
        public static Database[] listDB = null;
        public static SqlConnectionStringBuilder SqlConnectionStringBuilder = null;
        public static int TimeOutInSecond = 2;
        public static List<string> ListDBTemp = new List<string>();
        public static int NumberOfQuestion = 10;
    }
    public class Database
    {
        public Database(string sqlServerDbFolder, string sourceDbName)
        {
            SqlServerDbFolder = sqlServerDbFolder;
            SourceDbName = sourceDbName;
        }
        public string SqlServerDbFolder { get; set; }
        public string SourceDbName { get; set; }

    }
}
