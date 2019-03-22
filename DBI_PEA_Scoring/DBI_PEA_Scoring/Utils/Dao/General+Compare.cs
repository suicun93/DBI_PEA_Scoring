using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using DBI_PEA_Scoring.Common;
using DBI_PEA_Scoring.Model;
using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;

namespace DBI_PEA_Scoring.Utils.Dao
{
    public partial class General
    {
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
        public static Dictionary<string, string> CompareTwoDatabases(string db1Name, string db2Name, Candidate candidate, string errorMessage)
        {
            try
            {
                string compareQuery = "exec sp_CompareDb [" + db2Name + "], [" + db1Name + "]";
                var builder = Constant.SqlConnectionStringBuilder;
                string result = errorMessage;
                int count = 0;
                // Connect to SQL
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand commandCompare = new SqlCommand(compareQuery, connection))
                    {
                        commandCompare.CommandTimeout = Constant.TimeOutInSecond;
                        using (SqlDataReader reader = commandCompare.ExecuteReader())
                        {
                            result = string.Concat(result, "Check Table structure:\n", "||    Table Name    ||", "    Column Name    ||",
                                "    Data Type    ||", "    Nullable   ||\n");
                            while (reader.Read())
                            {
                                result = string.Concat(result, new string('-', 79), "\n",
                                    $"||{(string)reader["TABLENAME"],-18}||",
                                    $"{(string)reader["COLUMNNAME"],-19}||",
                                    $"{(string)reader["DATATYPE"],-17}||",
                                    $"{(string)reader["NULLABLE"],-15}||", "\n");
                                result = string.Concat(result, "\n");
                                count++;
                            }
                            reader.NextResult();
                            result = string.Concat(result, "Check Constraints:\n", "||    PK Table    ||",
                                "    PK Columns    ||", "    FK Table   ||", "    FK Columns   ||\n");
                            while (reader.Read())
                            {
                                result = string.Concat(result, new string('-', 76), "\n",
                                    $"||{(string)reader["FK_TABLE"],-16}||",
                                    $"{(string)reader["FK_COLUMNS"],-18}||",
                                    $"{(string)reader["PK_TABLE"],-15}||",
                                    $"{(string)reader["PK_COLUMNS"],-17}||", "\n");
                                result = string.Concat(result, "\n");
                                count++;
                            }
                            double point = candidate.Point - Constant.minusPoint * count;
                            result = string.Concat(result, count, " error(s) have been found\n");

                            point = point < 0 ? 0 : point;
                            result = point == candidate.Point ? "True" : result;
                            return new Dictionary<string, string>
                                {
                                    {"Point", point.ToString()},
                                    {"Comment", result}
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
                    return "Column Name wrong - " + dataTableSolutionSchema.Rows[i]["ColumnName"];
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
        public static Dictionary<string, string> CompareOneResultSet(string dbAnswerName, string dbSolutionName, string answer,
            Candidate candidate)
        {
            try
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

                    //Running answer query
                    try
                    {
                        using (var sqlCommandAnswer = new SqlCommand("USE " + dbAnswerName + "; \n" + answer + "", connection))
                        {
                            var sqlReaderAnswer = sqlCommandAnswer.ExecuteReader();
                            if (candidate.CheckColumnName) dataTableAnswerShema = sqlReaderAnswer.GetSchemaTable();
                            dataTableAnswer.Load(sqlReaderAnswer);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Answer query error: " + e.Message + "\n");
                    }
                    //Running Solution 
                    try
                    {
                        using (var sqlCommandSolution = new SqlCommand("USE " + dbSolutionName + "; \n" + candidate.Solution + "", connection))
                        {
                            var sqlReaderSolution = sqlCommandSolution.ExecuteReader();
                            if (candidate.CheckColumnName) dataTableSolutionShema = sqlReaderSolution.GetSchemaTable();
                            dataTableSolution.Load(sqlReaderSolution);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Solution query error: " + e.Message + "\n");
                    }

                    //Prepare point for testcases and data
                    double dataPoint = Math.Round(candidate.Point / 2, 2);

                    //Number of testcases
                    int numOfTc = 0;
                    if (candidate.RequireSort) numOfTc++;
                    if (candidate.CheckColumnName) numOfTc++;
                    if (candidate.CheckDistinct) numOfTc++;

                    double gradePoint = 0;//Grading Point
                    string comment = "";//Logs

                    //Point for each testcase passed
                    double tcPoint = Math.Round(candidate.Point / 2 / numOfTc, 2);

                    //Count testcases true
                    int tcCount = 0;

                    //STARTING FOR GRADING

                    //Check data using except only (Rows can be difference)
                    comment += "Compare Data: ";
                    if (CompareTwoDataSetsByExcept(dataTableAnswer, dataTableSolution))
                    {
                        gradePoint += dataPoint;
                        comment += string.Concat("Passed => +", dataPoint, "\n");

                        //1. Check if distinct is required
                        if (candidate.CheckDistinct)
                        {
                            comment += "- Check distinct: ";
                            //Compare number of rows
                            if (dataTableSolution.Rows.Count == dataTableAnswer.Rows.Count)
                            {
                                tcCount++;
                                comment += String.Concat("Passed => +", tcPoint, "\n");
                            }
                            else
                            {
                                comment += "Not pass\n";
                            }
                        }

                        //2. Check if sort is required
                        if (candidate.RequireSort)
                        {
                            comment += "- Check sort: ";
                            //Use distinct to remove all duplicate rows
                            string queryCheckSortAnswer = "SELECT DISTINCT* FROM (" + answer + ")";
                            string queryCheckSortSolution = "SELECT DISTINCT* FROM (" + candidate.Solution + ")";

                            DataTable dataTableSortAnswer = new DataTable();
                            DataTable dataTableSortSolution = new DataTable();

                            //Running Answer Sort query
                            try
                            {
                                using (var sqlCommandAnswer = new SqlCommand("USE " + dbAnswerName + "; \n" + queryCheckSortAnswer + "", connection))
                                {
                                    var sqlReaderAnswer = sqlCommandAnswer.ExecuteReader();
                                    dataTableSortAnswer.Load(sqlReaderAnswer);
                                }
                            }
                            catch (Exception e)
                            {
                                throw new Exception("Compare sort error at answer query: " + e.Message + "\n");
                            }
                            //Running Solution Sort query
                            try
                            {
                                using (var sqlCommandSolution = new SqlCommand("USE " + dbSolutionName + "; \n" + queryCheckSortSolution + "", connection))
                                {
                                    var sqlReaderSolution = sqlCommandSolution.ExecuteReader();
                                    dataTableSortSolution.Load(sqlReaderSolution);
                                }
                            }
                            catch (Exception e)
                            {
                                throw new Exception("Compare sort error at solution query: " + e.Message + "\n");
                            }

                            //Compare number of rows

                            if (dataTableSortSolution.Rows.Count == dataTableSortAnswer.Rows.Count)
                            {
                                tcCount++;
                                comment += string.Concat("Passed => +", tcPoint, "\n");
                            }
                            else
                            {
                                comment += "Not pass\n";
                            }
                        }
                        //3. Check if checkColumnName is required
                        if (candidate.CheckColumnName)
                        {
                            comment += "- Check Columns Name: ";
                            string resCompareColumnName =
                                CompareColumnsNameOfTables(dataTableAnswerShema, dataTableSolutionShema);
                            if (resCompareColumnName.Equals(""))
                            {
                                tcCount++;
                                comment += string.Concat("Passed => +", tcPoint, "\n");
                            }
                            else
                            {
                                comment += string.Concat(resCompareColumnName, "\n");
                            }
                        }
                    }
                    else
                    {
                        comment += "Not pass => Point = 0\n";
                    }
                    if (numOfTc == 0 || numOfTc == tcCount)
                    {
                        gradePoint = candidate.Point;
                    }
                    else
                    {
                        gradePoint = Math.Round(tcCount * tcPoint + gradePoint, 2);
                    }

                    return new Dictionary<string, string>
                {
                    {"Point", gradePoint.ToString()},
                    {"Comment", comment}
                };
                }
            }
            catch (Exception e)
            {
                throw new Exception("Compare error: " + e.Message);
            }
        }

        /// <summary>
        ///     Compare Multiple result set
        /// </summary>
        /// <param name="dbAnswerName">DB Name to check student query</param>
        /// <param name="dbSolutionName">DB Name to check teacher query</param>
        /// <param name="candidate">Candidate</param>
        /// <param name="errorMessage"></param>
        /// <returns>
        /// "true" if correct
        /// "false" if wrong
        /// message error from sqlserver if error
        /// </returns>
        public static Dictionary<string, string> CompareMoreResultSets(string dbAnswerName, string dbSolutionName,
            Candidate candidate, string errorMessage)
        {
            try
            {
                //Get testcases from comment in test query
                List<TestCase> testCases = GetTestCasesPoint(candidate.TestQuery);

                double totalPoint = 0;
                double maxPoint = candidate.Point;



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

                        int count = 0;
                        string comment = errorMessage;
                        //Compare results one by one
                        for (int i = 0; i < testCases.Count; i++)
                        {
                            foreach (DataTable tableAnswer in dataSetAnswer.Tables)
                                if (CompareTwoDataSetsByExcept(tableAnswer, dataSetSolution.Tables[i]))
                                {
                                    count++;
                                    totalPoint += testCases.ElementAt(i).Point;
                                    break;
                                }
                            comment = string.Concat("TC ", i, ": ", comment, testCases.ElementAt(i).Description, "\n");
                        }
                        //Degree 50% of point if Answer has more resultSets than Solution
                        if (dataSetSolution.Tables.Count < dataSetAnswer.Tables.Count)
                        {
                            maxPoint = Math.Round(maxPoint / 2, 2);
                            comment = string.Concat(comment,
                                "Decrease 50% of points because Answer has more resultSets than Solution (",
                                dataSetAnswer.Tables.Count, " > ", dataSetSolution.Tables.Count, ")\n");
                        }

                        totalPoint = totalPoint > maxPoint ? maxPoint : totalPoint;
                        if (count > 0)
                        {
                            return new Dictionary<string, string>
                            {
                                {"Point", totalPoint.ToString()},
                                {"Comment", string.Concat("Testcase passed: (", count, "/",testCases.Count ,") - ", (count == testCases.Count)&&(comment.Equals(""))? "True\n":comment)}
                            };
                        }
                        return new Dictionary<string, string>
                        {
                            {"Point", "0"},
                            {"Comment", string.Concat("Testcase passed: (", count, "/",testCases.Count ,") - ","Wrong Answer\n")}
                        };
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
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
        private static bool CompareTwoDataSetsByExcept(DataTable dataTableAnswer, DataTable dataTableSolution)
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
            if (dataTableSolution.Rows.Count != dataTableAnswer.Rows.Count ||
                dataTableSolution.Columns.Count != dataTableAnswer.Columns.Count) return false;
            for (int i = 0; i < dataTableSolution.Rows.Count; i++)
            {
                var rowArraySolution = dataTableSolution.Rows[i].ItemArray;
                var rowArrayAnswer = dataTableAnswer.Rows[i].ItemArray;
                if (!rowArraySolution.SequenceEqual(rowArrayAnswer))
                    return false;
            }
            return true;
        }
    }
}
