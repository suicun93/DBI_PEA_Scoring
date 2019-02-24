using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBI_PEA_Scoring.Common
{
    public class Constant
    {
        public static DB[] listDB = {new DB(@"DESKTOP-BA53MO4\SQLEXPRESS", "sa", "123", "master", @"C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\", "sample") };
    }
    public class DB
    {
        public DB(string dataSource, string userId, string password, string initialCatalog, string sqlServerDbFolder, string sourceDbName)
        {
            DataSource = dataSource;
            UserId = userId;
            Password = password;
            InitialCatalog = initialCatalog;
            SqlServerDbFolder = sqlServerDbFolder;
            SourceDbName = sourceDbName;
        }

        public string DataSource { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string InitialCatalog { get; set; }
        public string SqlServerDbFolder { get; set; }
        public string SourceDbName { get; set; }

    }
}
