using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System;

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
        public static bool CompareOneTableNoSort(string db1Name, string db2Name, string queryTable1,
            string queryTable2)
        {
            var builder = Common.Constant.SqlConnectionStringBuilder;
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
        public static bool CompareOneTableSort(string db1Name, string db2Name, string queryTable1,
            string queryTable2)
        {
            var builder = Common.Constant.SqlConnectionStringBuilder;
            //bool resCheckSchema = CompareOneTableNoSort(db1Name, db2Name, queryTable1, queryTable2);
            bool resCheckSchema = true;
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
                    return !TwoDataTableDifferenceDetector(dt1, dt2);
                }
            }
            return false;
        }
        /// <summary>
        ///     Compare Multiple result set
        /// </summary>
        /// <param name="dbNameStudent">DB Name to check student query</param>
        /// <param name="dbNameTeacher">DB Name to check teacher query</param>
        /// <param name="queryToCheck">Query to check from teacher</param>
        /// <returns>
        /// "true" if correct
        /// "false" if wrong
        /// message error from sqlserver if error
        /// </returns>
        public static bool CompareMoreThanOneTableSort(string dbNameStudent, string dbNameTeacher, string queryToCheck)
        {
            // Prepare Command
            var builder = Common.Constant.SqlConnectionStringBuilder;
            builder.MultipleActiveResultSets = true;
            string sqlStudent = "USE " + dbNameStudent + " \n" +
                         queryToCheck;
            string sqlTeacher = "USE " + dbNameTeacher + " \n" +
                         queryToCheck;
            // Connect and run query to check
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                // Prepare Command
                SqlCommand sqlCommandStudent = new SqlCommand(sqlStudent, connection);
                SqlCommand sqlCommandTeacher = new SqlCommand(sqlTeacher, connection);

                // Prepare SqlDataAdapter
                SqlDataAdapter adapterStudent = new SqlDataAdapter(sqlStudent, connection);
                SqlDataAdapter adapterTeacher = new SqlDataAdapter(sqlTeacher, connection);

                // Prepare DataSet
                DataSet dataSetStudent = new DataSet();
                DataSet dataSetTeacher = new DataSet();

                // Fill Data adapter to dataset
                adapterStudent.Fill(dataSetStudent);
                adapterTeacher.Fill(dataSetTeacher);
                ExecuteSingleQuery("Use master");
                
                // Check count of table of student and teacher is same or not.
                if (dataSetTeacher.Tables.Count > dataSetStudent.Tables.Count)
                    throw new Exception("Less table than teacher's requirement");
                else if (dataSetTeacher.Tables.Count < dataSetStudent.Tables.Count)
                    throw new Exception("More table than teacher's requirement");
                // If Number of table of student and teacher is same, then Compare one by one
                for (int i = 0; i < dataSetStudent.Tables.Count; i++)
                    if (TwoDataTableDifferenceDetector(dataSetStudent.Tables[i], dataSetTeacher.Tables[i]))
                    {
                        throw new Exception("Difference detected");
                    }
                return true;
            }
        }

        internal static bool CompareMoreThanOneTableSort(string dbTeacherName, string dbStudentName, string queryTeacher, string queryStudent)
        {
            // Prepare Command
            var builder = Common.Constant.SqlConnectionStringBuilder;
            builder.MultipleActiveResultSets = true;
            string sqlStudent = "USE " + dbStudentName + " \n" +
                         queryStudent;
            string sqlTeacher = "USE " + dbTeacherName + " \n" +
                         queryTeacher;
            // Connect and run query to check
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                // Prepare Command
                SqlCommand sqlCommandStudent = new SqlCommand(sqlStudent, connection);
                SqlCommand sqlCommandTeacher = new SqlCommand(sqlTeacher, connection);

                // Prepare SqlDataAdapter
                SqlDataAdapter adapterStudent = new SqlDataAdapter(sqlStudent, connection);
                SqlDataAdapter adapterTeacher = new SqlDataAdapter(sqlTeacher, connection);

                // Prepare DataSet
                DataSet dataSetStudent = new DataSet();
                DataSet dataSetTeacher = new DataSet();

                // Fill Data adapter to dataset
                adapterStudent.Fill(dataSetStudent);
                adapterTeacher.Fill(dataSetTeacher);
                ExecuteSingleQuery("Use master");

                // Check count of table of student and teacher is same or not.
                if (dataSetTeacher.Tables.Count > dataSetStudent.Tables.Count)
                    throw new Exception("Less table than teacher's requirement");
                else if (dataSetTeacher.Tables.Count < dataSetStudent.Tables.Count)
                    throw new Exception("More table than teacher's requirement");
                // If Number of table of student and teacher is same, then Compare one by one
                for (int i = 0; i < dataSetStudent.Tables.Count; i++)
                    if (TwoDataTableDifferenceDetector(dataSetStudent.Tables[i], dataSetTeacher.Tables[i]))
                    {
                        throw new Exception("Difference detected");
                    }
                return true;
            }
        }

        /// <summary>
        ///     Compare datatables same schema
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns>
        /// true = difference
        /// false = same
        /// </returns>
        public static bool TwoDataTableDifferenceDetector(DataTable dt1, DataTable dt2)
        {
            return dt1.AsEnumerable().Except(dt2.AsEnumerable(), DataRowComparer.Default).Any();
        }
    }
}
