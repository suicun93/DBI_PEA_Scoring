using System;
using System.Data.SqlClient;
using System.Data;
using System.Linq;

namespace DBI_PEA_Scoring.Utils.DaoType
{
    class SelectType
    {
        public SqlConnectionStringBuilder Builder { get; }
        public General General { get; }

        /// <summary>
        /// Init connection
        /// </summary>
        public SelectType(SqlConnectionStringBuilder builder)
        {
            // Build connection string
            Builder = builder;
            General = new General();
        }

        /// <summary>
        /// Marking Query Select type
        /// </summary>
        /// <param name="isSort">is this question need sorting</param>
        /// <param name="dbTeacherName"></param>
        /// <param name="dbStudentName"></param>
        /// <param name="queryTeacher"></param>
        /// <param name="queryStudent"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public bool MarkQueryType(bool isSort, string dbTeacherName, string dbStudentName, string queryTeacher,
            string queryStudent, SqlConnectionStringBuilder builder)
        {

            switch (isSort)
            {
                case true:
                    return CompareTableNoSort(dbTeacherName, dbStudentName, queryTeacher, queryStudent, builder);
                case false:
                    return CompareTableSort(dbTeacherName, dbStudentName, queryTeacher, queryStudent, builder);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Compare tables with sort
        /// </summary>
        /// <param name="db1Name"></param>
        /// <param name="db2Name"></param>
        /// <param name="queryTable1"></param>
        /// <param name="queryTable2"></param>
        /// <param name="builder"></param>
        /// <returns>
        /// "true" if correct
        /// "false" if wrong
        /// message error from sqlserver if error
        /// </returns>
        private bool CompareTableSort(string db1Name, string db2Name, string queryTable1,
            string queryTable2, SqlConnectionStringBuilder builder)
        {
            bool resCheckSchema = CompareTableNoSort(db1Name, db2Name, queryTable1, queryTable2, builder);
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
                    if (CompareDataTables(dt1, dt2))
                    {
                        return false;
                    }
                    return true;
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
        private bool CompareDataTables(DataTable dt1, DataTable dt2)
        {
            return dt1.AsEnumerable().Except(dt2.AsEnumerable(), DataRowComparer.Default).Any();
        }


        ////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Compare Table no sort
        /// </summary>
        /// <param name="db1Name"></param>
        /// <param name="db2Name"></param>
        /// <param name="queryTable1"></param>
        /// <param name="queryTable2"></param>
        /// <param name="builder"></param>
        /// //name host (localhost)
        /// <returns>
        /// "true" if correct
        /// "false" if wrong
        /// message error from sqlserver if error
        /// </returns>
        public bool CompareTableNoSort(string db1Name, string db2Name, string queryTable1,
            string queryTable2, SqlConnectionStringBuilder builder)
        {
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
                        if (command.ExecuteScalar() == null)
                        {
                            return true;
                        }
                        return false;
                    }
                }
                
        }
    }
}
