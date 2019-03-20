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
        public static Dictionary<string, string> CompareTwoDatabases(string db1Name, string db2Name, Candidate candidate)
        {
            try
            {
                string compareQuery = "exec sp_CompareDb [" + db2Name + "], [" + db1Name + "]";
                var builder = Constant.SqlConnectionStringBuilder;
                // Connect to SQL
                string result = "";
                int count = 0;
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
                                result = string.Concat(result, new string('-', 79), "\n", string.Format("||{0,-18}||", (string)reader["TABLENAME"]),
                                    string.Format("{0,-19}||", (string)reader["COLUMNNAME"]),
                                    string.Format("{0,-17}||", (string)reader["DATATYPE"]),
                                    string.Format("{0,-15}||", (string)reader["NULLABLE"]), "\n");
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
                        throw new Exception("Answer error: e.Message");
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
                    if (candidate.CheckDistinct) numOfTc++;
                    double pointCheckData = candidate.Point / 2;
                    double pointEachTc = Math.Round(candidate.Point / 2 / numOfTc, 2);
                    int count = 0;
                    //check data using except only (rows can be difference
                    if (CompareTwoDataSetsByData(dataTableAnswer, dataTableSolution, false))
                    {
                        count++;
                        point += pointCheckData;

                        //Distinct and Sort are independence. They cant both equal true.
                        //If check distinct is require
                        if (candidate.CheckDistinct)
                        {
                            if (dataTableSolution.Rows.Count == dataTableAnswer.Rows.Count)
                            {
                                count++;
                                point += pointEachTc;
                            }
                            else
                            {
                                comment = string.Concat(comment, "Distinct is required!\n");
                            }
                        }

                        //If Sort is require
                        if (candidate.RequireSort)
                        {
                            if (CompareTwoDataSetsByRow(dataTableAnswer, dataTableSolution))
                            {
                                count++;
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
                                count++;
                            }
                            else
                            {
                                comment = string.Concat(comment, resCompareColumnName);
                            }
                        }
                        point += count * pointEachTc;
                    }
                    //Wrong query
                    else
                    {
                        point = 0;
                        comment = "Wrong Answer\n";
                    }
                    if (comment.Equals(""))
                    {
                        comment = "True\n";
                        point = candidate.Point;
                    }
                    return new Dictionary<string, string>
                {
                    {"Point", point.ToString()},
                    {"Comment", string.Concat("Testcase passed: (", count, "/",numOfTc ,") - ",comment)}
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
                        double pointEachTc = Math.Round(candidate.Point / numOfTc, 2);
                        int count = 0;
                        string comment = errorMessage;
                        //Compare results one by one
                        for (int i = 0; i < numOfTc; i++)
                        {
                            foreach (DataTable tableAnswer in dataSetAnswer.Tables)
                                if (CompareTwoDataSetsByData(tableAnswer, dataSetSolution.Tables[i], true))
                                {
                                    count++;
                                    break;
                                }
                        }
                        //Degree 50% of point if Answer has more resultSets than Solution
                        if (dataSetSolution.Tables.Count < dataSetAnswer.Tables.Count)
                        {
                            pointEachTc = Math.Round(pointEachTc / 2, 2);
                            comment = string.Concat(comment, "Decrease 50% of points because Answer has more resultSets than Solution (",
                                dataSetAnswer.Tables.Count, " > ", dataSetSolution.Tables.Count, ")\n");
                        }
                        if (count > 0)
                        {
                            return new Dictionary<string, string>
                            {
                                {"Point", (count * pointEachTc).ToString()},
                                {"Comment", string.Concat("Testcase passed: (", count, "/",numOfTc ,") - ", (count == numOfTc)&&(comment.Equals(""))? "True\n":
                                string.Concat("Wrong Answer\n", comment))}
                            };
                        }
                        return new Dictionary<string, string>
                        {
                            {"Point", "0"},
                            {"Comment", string.Concat("Testcase passed: (", count, "/",numOfTc ,") - ","Wrong Answer\n")}
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
        private static bool CompareTwoDataSetsByData(DataTable dataTableAnswer, DataTable dataTableSolution, bool checkRowsAndColumns)
        {
            if (checkRowsAndColumns) if (dataTableSolution.Rows.Count != dataTableAnswer.Rows.Count ||
                               dataTableSolution.Columns.Count != dataTableAnswer.Columns.Count) return false;
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
