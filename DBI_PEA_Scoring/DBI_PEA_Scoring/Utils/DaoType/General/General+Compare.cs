using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System;
using DBI_PEA_Scoring.Common;
using DBI_PEA_Scoring.Model;

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
        /// Compare Columns Name of tables
        /// </summary>
        /// <param name="db1Name">db1 Name</param>
        /// <param name="db2Name">db2 Name</param>
        /// <param name="queryTable1">query to select table1</param>
        /// <param name="queryTable2">query to select table2</param>
        /// <returns>"" if true, "(comment)" if false</returns>
        public static string CompareOneTableColumnsName(string db1Name, string db2Name, string queryTable1, string queryTable2)
        {
            // Connect to SQL
            var tableSchema1 = new DataTable();
            var tableSchema2 = new DataTable();
            var builder = Constant.SqlConnectionStringBuilder;
            // Connect to SQL
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("USE " + db1Name + "; \n" + queryTable1 + "", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        tableSchema1 = reader.GetSchemaTable();
                    }
                }
                using (SqlCommand command = new SqlCommand("USE " + db2Name + "; \n" + queryTable2 + "", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        tableSchema2 = reader.GetSchemaTable();
                    }
                }
            }
            if (tableSchema1.Rows.Count != tableSchema2.Rows.Count)
            {
                return "Number of Columns is wrong: " + tableSchema1.Rows.Count + "";
            }
            else
            {
                for (int i = 0; i < tableSchema1.Rows.Count; i++)
                {
                    if (!tableSchema1.Rows[i]["ColumnName"].Equals(tableSchema2.Rows[i]["ColumnName"]))
                    {
                        return "Column Name wrong: " + tableSchema1.Rows[i]["ColumnName"] + "";
                    }
                }
            }
            return "";
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
        public static bool CompareOneTable(string db1Name, string db2Name, string queryTable1, string queryTable2)
        {
            var builder = Constant.SqlConnectionStringBuilder;
            // Connect to SQL
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                DataTable dt1 = new DataTable();
                DataTable dt2 = new DataTable();
                using (SqlCommand command = new SqlCommand("USE " + db1Name + "; \n" + queryTable1 + "", connection))
                {
                    SqlDataReader reader1 = command.ExecuteReader();
                    dt1.Load(reader1);
                }
                using (SqlCommand command = new SqlCommand("USE " + db2Name + "; \n" + queryTable2 + "", connection))
                {
                    SqlDataReader reader2 = command.ExecuteReader();
                    dt2.Load(reader2);
                }
                return !TwoDataTableDifferenceDetector(dt1, dt2);
            }
        }


        /// <summary>
        ///     Compare Multiple result set
        /// </summary>
        /// <param name="dbAnswerName">DB Name to check student query</param>
        /// <param name="dbSolutionName">DB Name to check teacher query</param>
        /// <param name="queryToCheck">Query to check from teacher</param>
        /// <returns>
        /// "true" if correct
        /// "false" if wrong
        /// message error from sqlserver if error
        /// </returns>
        public static bool CompareMoreThanOneTableSort(string dbAnswerName, string dbSolutionName, string queryToCheck)
        {
            // Prepare Command
            var builder = Constant.SqlConnectionStringBuilder;
            builder.MultipleActiveResultSets = true;
            string sqlStudent = "USE " + dbAnswerName + " \n" +
                         queryToCheck;
            string sqlTeacher = "USE " + dbSolutionName + " \n" +
                         queryToCheck;
            // Connect and run query to check
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                // Prepare Command
                SqlCommand sqlCommandStudent = new SqlCommand(sqlStudent, connection);
                SqlCommand sqlCommandTeacher = new SqlCommand(sqlTeacher, connection);
                sqlCommandStudent.CommandTimeout = Constant.TimeOutInSecond;
                sqlCommandTeacher.CommandTimeout = Constant.TimeOutInSecond;

                // Prepare SqlDataAdapter
                SqlDataAdapter adapterStudent = new SqlDataAdapter(sqlStudent, connection);
                SqlDataAdapter adapterTeacher = new SqlDataAdapter(sqlTeacher, connection);

                // Prepare DataSet
                DataSet dataSetStudent = new DataSet();
                DataSet dataSetTeacher = new DataSet();

                // Fill Data adapter to dataset
                try
                {
                    adapterStudent.Fill(dataSetStudent);
                }
                catch (Exception e)
                {
                    throw new Exception("Answer error: " + e.Message);
                }
                try
                {
                    adapterTeacher.Fill(dataSetTeacher);
                }
                catch (Exception e)
                {
                    throw new Exception("Solution error: " + e.Message);
                }
                connection.Close();

                // Check count of table of student and teacher is same or not.
                if (dataSetTeacher.Tables.Count > dataSetStudent.Tables.Count)
                    throw new Exception("Less table than requirement");
                else if (dataSetTeacher.Tables.Count < dataSetStudent.Tables.Count)
                    throw new Exception("More table than requirement");
                // If Number of table of student and teacher is same, then Compare one by one
                for (int i = 0; i < dataSetStudent.Tables.Count; i++)
                    if (TwoDataTableDifferenceDetector(dataSetStudent.Tables[i], dataSetTeacher.Tables[i]))
                    {
                        throw new Exception("Result Not matched");
                    }
                return true;
            }
        }

        internal static bool CompareMoreTables(string dbAnswerName, string dbSolutionName, string answer, Candidate candidate)
        {
            // Prepare Command
            var builder = Constant.SqlConnectionStringBuilder;
            builder.MultipleActiveResultSets = true;
            string queryAnswer = "USE " + dbAnswerName + " \n" + answer;
            string querySolution = "USE " + dbSolutionName + " \n" + candidate.Solution;
            // Connect and run query to check
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                // Prepare Command
                SqlCommand sqlCommandStudent = new SqlCommand(queryAnswer, connection);
                SqlCommand sqlCommandTeacher = new SqlCommand(querySolution, connection);
                sqlCommandStudent.CommandTimeout = Constant.TimeOutInSecond;
                sqlCommandTeacher.CommandTimeout = Constant.TimeOutInSecond;

                // Prepare SqlDataAdapter
                SqlDataAdapter adapterStudent = new SqlDataAdapter(queryAnswer, connection);
                SqlDataAdapter adapterTeacher = new SqlDataAdapter(querySolution, connection);

                // Prepare DataSet
                DataSet dataSetStudent = new DataSet();
                DataSet dataSetTeacher = new DataSet();

                // Fill Data adapter to dataset
                try
                {
                    adapterStudent.Fill(dataSetStudent);
                }
                catch (Exception e)
                {
                    throw new Exception("Answer error: " + e.Message);
                }
                try
                {
                    adapterTeacher.Fill(dataSetTeacher);
                }
                catch (Exception e)
                {
                    throw new Exception("Solution error: " + e.Message);
                }
                connection.Close();

                // Check count of table of student and teacher is same or not.
                if (dataSetTeacher.Tables.Count > dataSetStudent.Tables.Count)
                    throw new Exception("Less table than requirement");
                else 
                if (dataSetTeacher.Tables.Count < dataSetStudent.Tables.Count)
                    throw new Exception("More table than requirement");
                // If Number of table of student and teacher is same, then Compare one by one
                for (int i = 0; i < dataSetStudent.Tables.Count; i++)
                {
                    if (TwoDataTableDifferenceDetector(dataSetStudent.Tables[i], dataSetTeacher.Tables[i]))
                    {
                        throw new Exception("Result Not matched");
                    }
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
