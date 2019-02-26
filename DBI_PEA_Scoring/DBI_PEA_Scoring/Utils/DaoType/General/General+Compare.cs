using DBI_PEA_Scoring.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DBI_PEA_Scoring.Utils.DaoType
{
    public partial class General
    {
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
