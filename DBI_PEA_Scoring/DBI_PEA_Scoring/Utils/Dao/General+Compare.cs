using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using DBI_PEA_Scoring.Common;
using DBI_PEA_Scoring.Model;

namespace DBI_PEA_Scoring.Utils.Dao
{
    public partial class General
    {
        public static bool PrepareSpCompareDatabase()
        {
            var builder = Constant.SqlConnectionStringBuilder;
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                //Check exists SP sp_compareDb
                try
                {
                    ExecuteSingleQuery(SchemaType.ProcCompareDbsCreate, "master");
                }
                catch (Exception e)
                {
                    throw new Exception("Compare error: " + e.Message);
                }
                try
                {
                    ExecuteSingleQuery(SchemaType.ProcCompareDbsAlter, "master");
                }
                catch (Exception e)
                {
                    throw new Exception("Compare error: " + e.Message);
                }
                return true;
            }
        }

        /// <summary>
        /// Compare 2 databases
        /// </summary>
        /// <param name="db1Name">Student Database Name</param>
        /// <param name="db2Name">Solution Database Name</param>
        /// <param name="candidate"></param>
        /// <returns>
        /// "true" if correct
        /// "false" if wrong
        /// message error from sqlserver if error</returns>
        public static Dictionary<string, string> CompareTwoDatabases(string db1Name, string db2Name, Candidate candidate)
        {
            try
            {
                string compareQuery = "exec sp_CompareDb [" + db2Name + "], [" + db1Name + "]";
                var builder = Constant.SqlConnectionStringBuilder;
                // Connect to SQL
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand commandCompare = new SqlCommand(compareQuery, connection))
                    {
                        using (SqlDataReader reader = commandCompare.ExecuteReader())
                        {
                            string result = string.Concat("Check Table structure:\n", "Table Name\t",
                                "Column Name\t", "Data Type\t", "Nullable\n");
                            double point = candidate.Point;
                            while (reader.Read())
                            {
                                result = string.Concat(result, (string)reader["TABLENAME"], "\t", (string)reader["COLUMNNAME"], "\t",
                                    (string)reader["DATATYPE"], "\t", (string)reader["NULLABLE"], "\n");
                                point -= Constant.minusPoint;
                            }
                            reader.NextResult();
                            while (reader.Read())
                            {
                                result = string.Concat(result, "Check Constraints:\n", "FK Table\t",
                                    "FK Columns\t", "PK Table\t", "PK Columns\n");
                                result = string.Concat(result, (string)reader["FK_TABLE"], "\t", (string)reader["FK_COLUMNS"], "\t",
                                    (string)reader["PK_TABLE"], "\t", (string)reader["PK_COLUMNS"], "\n");
                                point -= Constant.minusPoint;
                            }
                            point = point < 0 ? 0 : point;
                            result = point == candidate.Point ? "True" : result;
                            return new Dictionary<string, string>
                                {
                                    {"Point", point.ToString()},
                                    {"Comment", result},
                                };
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Compare error: " + ex.Message);
            }

        }

        /// <summary>
        /// Compare Columns Name of tables
        /// </summary>
        /// <param name="dataTableAnswerSchema"></param>
        /// <param name="dataTableSolutionSchema"></param>
        /// <returns>"(Empty)" if true, "(comment)" if false</returns>
        public static string CompareColumnsNameOfTables(DataTable dataTableAnswerSchema, DataTable dataTableSolutionSchema)
        {
            for (var i = 0; i < dataTableSolutionSchema.Rows.Count; i++)
            {
                if (!dataTableSolutionSchema.Rows[i]["ColumnName"].Equals(dataTableAnswerSchema.Rows[i]["ColumnName"]))
                {
                    return "ColumnName wrong: " + dataTableSolutionSchema.Rows[i]["ColumnName"] + "\n";
                }
            }
            return "";
        }

        /// <summary>
        /// Compare tables with sort
        /// </summary>
        /// <param name="dbAnswerName"></param>
        /// <param name="dbSolutionName"></param>
        /// <param name="answer"></param>
        /// <param name="candidate"></param>
        /// <returns>
        /// "true" if correct
        /// "false" if wrong
        /// message error from sqlserver if error
        /// </returns>
        public static Dictionary<string, string> CompareOneResultSet(string dbAnswerName, string dbSolutionName, string answer, Candidate candidate)
        {
            var builder = Constant.SqlConnectionStringBuilder;
            DataTable dataTableAnswer = new DataTable();
            DataTable dataTableSolution = new DataTable();
            DataTable dataTableAnswerShema = new DataTable();
            DataTable dataTableSolutionShema = new DataTable();

            // Connect to SQL
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (var sqlCommandAnswer = new SqlCommand("USE " + dbAnswerName + "; \n" + answer + "", connection))
                {
                    var sqlReaderAnswer = sqlCommandAnswer.ExecuteReader();
                    if (candidate.CheckColumnName) dataTableAnswerShema = sqlReaderAnswer.GetSchemaTable();
                    dataTableAnswer.Load(sqlReaderAnswer);
                }
                using (var sqlCommandSolution = new SqlCommand("USE " + dbSolutionName + "; \n" + candidate.Solution + "", connection))
                {
                    var sqlReaderSolution = sqlCommandSolution.ExecuteReader();
                    if (candidate.CheckColumnName) dataTableSolutionShema = sqlReaderSolution.GetSchemaTable();
                    dataTableSolution.Load(sqlReaderSolution);
                }
                double point = 0;
                string comment = "";
                int numOfTc = 1;
                if (candidate.RequireSort) numOfTc++;
                if (candidate.CheckColumnName) numOfTc++;
                double pointEachTc = Math.Round(candidate.Point / numOfTc, 2);
                //check data
                if (CompareTwoDataSetsByData(dataTableAnswer, dataTableSolution))
                {
                    point += pointEachTc;
                    //If Sort is require
                    if (candidate.RequireSort)
                    {
                        if (CompareTwoDataSetsByRow(dataTableAnswer, dataTableSolution))
                        {
                            point += pointEachTc;
                            //comment = "False\n";
                        }
                        else
                        {
                            comment = string.Concat(comment, "Result is not sorted as required!\n");
                        }
                    }

                    //If check columns name is require
                    if (candidate.CheckColumnName)
                    {
                        string resCompareColumnName =
                            CompareColumnsNameOfTables(dataTableAnswerShema, dataTableSolutionShema);
                        if (resCompareColumnName.Equals(""))
                        {
                            point += pointEachTc;

                        }
                        else
                        {
                            comment = string.Concat(comment, resCompareColumnName);
                        }
                    }
                }
                //Wrong query
                else
                {
                    point = 0;
                    comment = "False\n";
                }
                if (comment.Equals(""))
                {
                    comment = "True\n";
                    point = candidate.Point;
                }
                return new Dictionary<string, string>
                {
                    {"Point", point.ToString()},
                    {"Comment", comment + "\n"}
                };
            }
        }

        /// <summary>
        ///     Compare Multiple result set
        /// </summary>
        /// <param name="dbAnswerName">DB Name to check student query</param>
        /// <param name="dbSolutionName">DB Name to check teacher query</param>
        /// <param name="candidate">Candidate</param>
        /// <returns>
        /// "true" if correct
        /// "false" if wrong
        /// message error from sqlserver if error
        /// </returns>
        public static Dictionary<string, string> CompareMoreResultSets(string dbAnswerName, string dbSolutionName, Candidate candidate)
        {
            // Prepare Command
            var builder = Constant.SqlConnectionStringBuilder;
            builder.MultipleActiveResultSets = true;
            var queryAnswer = "USE " + dbAnswerName + " \n" + candidate.TestQuery;
            var querySolution = "USE " + dbSolutionName + " \n" + candidate.TestQuery;
            // Connect and run query to check
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                // Prepare Command

                // Prepare SqlDataAdapter
                using (var sqlDataAdapterAnswer = new SqlDataAdapter(queryAnswer, connection))
                using (var sqlDataAdapterSolution = new SqlDataAdapter(querySolution, connection))
                {
                    // Prepare DataSet
                    var dataSetAnswer = new DataSet();
                    var dataSetSolution = new DataSet();

                    // Fill Data adapter to dataset
                    sqlDataAdapterAnswer.Fill(dataSetAnswer);
                    sqlDataAdapterSolution.Fill(dataSetSolution);

                    int numOfTc = dataSetSolution.Tables.Count;
                    double pointEachTc = candidate.Point / numOfTc;
                    int count = 0;
                    string comment = "";
                    // If Number of table of student and teacher is same, then Compare one by one
                    for (int i = 0; i < numOfTc; i++)
                    {
                        foreach (DataTable tableAnswer in dataSetAnswer.Tables)
                            if (CompareTwoDataSetsByData(tableAnswer, dataSetSolution.Tables[i]))
                            {
                                count++;
                                comment = string.Concat(comment, "Testcase ", i + 1, ": True\n");
                                break;
                            }
                            else if (i + 1 == numOfTc)
                                comment = string.Concat(comment, "Testcase ", i + 1, ": False\n");
                    }

                    if (count > 0)
                    {
                        return new Dictionary<string, string>
                            {
                                {"Point", (count * pointEachTc).ToString(CultureInfo.InvariantCulture)},
                                {"Comment", comment}
                            };
                    }
                    return new Dictionary<string, string>
                        {
                            {"Point", "0"},
                            {"Comment", "False\n"}
                        };

                }
            }
        }

        /// <summary>
        ///     Compare data 
        /// </summary>
        /// <param name="dataTableAnswer"></param>
        /// <param name="dataTableSolution"></param>
        /// <returns>
        /// true = same
        /// </returns>
        private static bool CompareTwoDataSetsByData(DataTable dataTableAnswer, DataTable dataTableSolution)
        {
            return !dataTableAnswer.AsEnumerable().Except(dataTableSolution.AsEnumerable(), DataRowComparer.Default).Any();
        }

        /// <summary>
        ///     Compare data line by row
        /// </summary>
        /// <param name="dataTableAnswer"></param>
        /// <param name="dataTableSolution"></param>
        /// <returns>
        /// true = same
        /// </returns>
        private static bool CompareTwoDataSetsByRow(DataTable dataTableAnswer, DataTable dataTableSolution)
        {
            if (dataTableAnswer.Rows.Count != dataTableSolution.Rows.Count || dataTableAnswer.Columns.Count != dataTableSolution.Rows.Count)
                return false;
            for (int i = 0; i < dataTableSolution.Rows.Count; i++)
            {
                var rowArraySolution = dataTableSolution.Rows[i].ItemArray;
                var rowArrayAnswer = dataTableAnswer.Rows[i].ItemArray;
                if (!rowArraySolution.SequenceEqual(rowArrayAnswer))
                    return false;
            }
            return true;
        }

        //internal static bool CompareMoreResultSets(string dbAnswerName, string dbSolutionName, string answer, Candidate candidate)
        //{
        //    // Prepare Command
        //    var builder = Constant.SqlConnectionStringBuilder;
        //    builder.MultipleActiveResultSets = true;
        //    string queryAnswer = "USE " + dbAnswerName + " \n" + answer;
        //    string querySolution = "USE " + dbSolutionName + " \n" + candidate.Solution;
        //    // Connect and run query to check
        //    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
        //    {
        //        connection.Open();
        //        // Prepare Command
        //        SqlCommand sqlCommandStudent = new SqlCommand(queryAnswer, connection);
        //        SqlCommand sqlCommandTeacher = new SqlCommand(querySolution, connection);
        //        sqlCommandStudent.CommandTimeout = Constant.TimeOutInSecond;
        //        sqlCommandTeacher.CommandTimeout = Constant.TimeOutInSecond;

        //        // Prepare SqlDataAdapter
        //        SqlDataAdapter adapterStudent = new SqlDataAdapter(queryAnswer, connection);
        //        SqlDataAdapter adapterTeacher = new SqlDataAdapter(querySolution, connection);

        //        // Prepare DataSet
        //        DataSet dataSetStudent = new DataSet();
        //        DataSet dataSetTeacher = new DataSet();

        //        // Fill Data adapter to dataset
        //        try
        //        {
        //            adapterStudent.Fill(dataSetStudent);
        //        }
        //        catch (Exception e)
        //        {
        //            throw new Exception("Answer error: " + e.Message);
        //        }
        //        try
        //        {
        //            adapterTeacher.Fill(dataSetTeacher);
        //        }
        //        catch (Exception e)
        //        {
        //            throw new Exception("Solution error: " + e.Message);
        //        }
        //        connection.Close();

        //        // Check count of table of student and teacher is same or not.
        //        if (dataSetTeacher.Tables.Count > dataSetStudent.Tables.Count)
        //            throw new Exception("Less table than requirement");
        //        if (dataSetTeacher.Tables.Count < dataSetStudent.Tables.Count)
        //            throw new Exception("More table than requirement");
        //        // If Number of table of student and teacher is same, then Compare one by one
        //        for (int i = 0; i < dataSetStudent.Tables.Count; i++)
        //        {
        //            if (CompareTwoDataSetsNoSort(dataSetStudent.Tables[i], dataSetTeacher.Tables[i]))
        //            {
        //                throw new Exception("Result Not matched");
        //            }
        //        }
        //        return true;
        //    }
        //}


    }
}
