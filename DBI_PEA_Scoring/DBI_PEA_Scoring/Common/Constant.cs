using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DBI_PEA_Scoring.Common
{
    public class Constant
    {
        public static Database[] listDB = null;
        public static SqlConnectionStringBuilder SqlConnectionStringBuilder = null;
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
