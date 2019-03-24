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
        /// <param name="dbAnswerName">Student Database Name</param>
        /// <param name="dbSolutionName">Solution Database Name</param>
        /// <param name="dbEmptyName"></param>
        /// <param name="candidate"></param>
        /// <param name="errorMessage"></param>
        /// <returns>
        /// "true" if correct
        /// "false" if wrong
        /// message error from sqlserver if error</returns>
        public static Dictionary<string, string> CompareTwoDatabases(string dbAnswerName, string dbSolutionName, string dbEmptyName, Candidate candidate, string errorMessage)
        {
            try
            {
                //Prepare query
                string compareQuery = "exec sp_CompareDb [" + dbSolutionName + "], [" + dbAnswerName + "]";
                string countComparisonQuery = "exec sp_CompareDb [" + dbSolutionName + "], [" + dbEmptyName + "]";
                //Comment result
                string comment = errorMessage;

                //1. Count tables
                int countAnswerTables = GetNumberOfTablesInDatabase(dbAnswerName);
                int countSolutionTables = GetNumberOfTablesInDatabase(dbSolutionName);

                if (countAnswerTables <= 0)
                {
                    return new Dictionary<string, string>
                    {
                        {"Point", "0"},
                        {"Comment", comment}
                    };
                }

                //Decrease maxpoint by rate
                double ratePoint = 1;
                //Max point
                double maxPoint = candidate.Point;
                comment += "Count Tables in database: ";
                if (countAnswerTables > countSolutionTables)
                {
                    ratePoint = (double)countSolutionTables / countAnswerTables;
                    maxPoint = Math.Round((double)candidate.Point * ratePoint, 2);
                    comment += string.Concat("Answer has more columns than Solution database (", countAnswerTables, ">",
                        countSolutionTables, ") => Decrease Max Point by ", ratePoint * 100, "% => Max Point = ", maxPoint, "\n");
                }
                else if (countSolutionTables > countAnswerTables)
                {
                    ratePoint = (double)countAnswerTables / countSolutionTables;
                    maxPoint = Math.Round((double)candidate.Point * ratePoint, 2);
                    comment += string.Concat("Answer has less columns than Solution database (", countAnswerTables, "<",
                        countSolutionTables, ") => Decrease Max Point by ", ratePoint * 100, "% => Max Point = ", maxPoint, "\n");
                }
                else
                {
                    comment += "Same\n";
                }
                //Count Comparison
                int numOfComparison;
                using (DataSet dtsNumOfComparison = GetDataSetFromReader(countComparisonQuery))
                {
                    numOfComparison = dtsNumOfComparison.Tables[0].Rows.Count * 2 + dtsNumOfComparison.Tables[1].Rows.Count;
                }
                //Get Dataset compare result
                using (DataSet dtsCompare = GetDataSetFromReader(compareQuery))
                {
                    if (dtsCompare == null)
                        throw new Exception("Compare error");
                    //Get all errors
                    var errorsConstructureRows = dtsCompare.Tables[0].AsEnumerable()
                        .Where(myRow => myRow.Field<string>("DATABASENAME").Contains("Solution"));
                    var errorsConstraintRows = dtsCompare.Tables[1].AsEnumerable()
                        .Where(myRow => myRow.Field<string>("DATABASENAME").Contains("Solution"));
                    //Sumary point
                    int totalErros = errorsConstraintRows.Count() + errorsConstructureRows.Count();
                    double gradePoint = Math.Round(maxPoint * (numOfComparison - totalErros) / numOfComparison, 2);
                    comment += string.Concat("Correct ", (numOfComparison - totalErros), "/", numOfComparison,
                        " comparison => Point = ", gradePoint, totalErros != 0 ? ", details:" : "", "\n");

                    //Details
                    //About constructure
                    if (errorsConstructureRows.Any())
                    {
                        comment += "- Constructure check:\n";
                        //Separate error:
                        //Definition not matching
                        var defErrors = errorsConstructureRows.Where(myRow =>
                            myRow.Field<string>("REASON").Equals("Definition not matching"));
                        //Missing column
                        var missingErrors = errorsConstructureRows.Where(myRow =>
                            myRow.Field<string>("REASON").Equals("Missing column or Wrong column name") && myRow.Field<string>("DATABASENAME").Contains("Solution"));
                        if (defErrors.Any())
                        {
                            comment += "+ Definition not matching:\n";
                            foreach (var rowSolution in defErrors)
                            {
                                comment += string.Concat("    Required: ", rowSolution["TABLENAME"], "(", rowSolution["COLUMNNAME"], ") => ", rowSolution["DATATYPE"], ", ", rowSolution["NULLABLE"], "\n");
                                var rowAnswer = dtsCompare.Tables[0].AsEnumerable().Where(myRow =>
                                    myRow.Field<string>("REASON").Equals("Definition not matching") &&
                                    myRow.Field<string>("COLUMNNAME").ToLower()
                                        .Equals(rowSolution["COLUMNNAME"].ToString().ToLower())
                                    && myRow.Field<string>("TABLENAME").ToLower()
                                        .Equals(rowSolution["TABLENAME"].ToString().ToLower()) && myRow.Field<string>("DATABASENAME").Contains("Answer")).ElementAt(0);
                                comment += string.Concat("    Answer  : ", rowAnswer["TABLENAME"], "(", rowAnswer["COLUMNNAME"], ") => ", rowAnswer["DATATYPE"], ", ", rowAnswer["NULLABLE"], "\n");
                            }
                        }
                        if (missingErrors.Any())
                        {
                            comment += "+ Column missing: ";
                            foreach (var rowSolution in missingErrors)
                            {
                                comment += string.Concat(rowSolution["COLUMNNAME"], "(", rowSolution["TABLENAME"], "), ");
                            }
                            comment = string.Concat(comment.Remove(comment.Length - 2), "\n");
                        }
                    }
                    //About Constraints
                    if (errorsConstraintRows.Any())
                    {
                        comment += "- Constraints check:\n";
                        foreach (var rowSolution in errorsConstraintRows)
                        {
                            comment += string.Concat("  Missing ", rowSolution["PK_COLUMNS"], "(", rowSolution["PK_TABLE"], ") - ", rowSolution["FK_COLUMNS"], "(", rowSolution["FK_TABLE"], ")\n");
                        }
                    }
                    return new Dictionary<string, string>
                {
                    {"Point", gradePoint.ToString()},
                    {"Comment", comment}
                };
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
            for (int i = 0; i < dataTableSolutionSchema.Rows.Count; i++)
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
                SqlConnectionStringBuilder builder = Constant.SqlConnectionStringBuilder;
                DataTable dataTableAnswer = new DataTable();
                DataTable dataTableSolution = new DataTable();
                DataTable dataTableAnswerShema = new DataTable();
                DataTable dataTableSolutionShema = new DataTable();

                // Connect to SQL
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();

                    //Running answer query
                    try
                    {
                        using (SqlCommand sqlCommandAnswer = new SqlCommand("USE " + dbAnswerName + ";\n" + answer + "", connection))
                        {
                            sqlCommandAnswer.CommandTimeout = Constant.TimeOutInSecond;
                            SqlDataReader sqlReaderAnswer = sqlCommandAnswer.ExecuteReader();
                            if (candidate.CheckColumnName) dataTableAnswerShema = sqlReaderAnswer.GetSchemaTable();
                            dataTableAnswer.Load(sqlReaderAnswer);
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    //Running Solution 
                    try
                    {
                        using (SqlCommand sqlCommandSolution = new SqlCommand("USE " + dbSolutionName + ";\n" + candidate.Solution + "", connection))
                        {
                            sqlCommandSolution.CommandTimeout = Constant.TimeOutInSecond;
                            SqlDataReader sqlReaderSolution = sqlCommandSolution.ExecuteReader();
                            if (candidate.CheckColumnName) dataTableSolutionShema = sqlReaderSolution.GetSchemaTable();
                            dataTableSolution.Load(sqlReaderSolution);
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    //Number of testcases
                    int numOfTc = 0;
                    if (candidate.RequireSort) numOfTc++;
                    if (candidate.CheckColumnName) numOfTc++;
                    if (candidate.CheckDistinct) numOfTc++;

                    //Prepare point for testcases and data
                    double dataPoint = numOfTc != 0 ? Math.Round(candidate.Point / 2, 2) : candidate.Point;

                    double gradePoint = 0;//Grading Point
                    string comment = "";//Logs

                    //Point for each testcase passed
                    double tcPoint = numOfTc > 0 ? Math.Round(candidate.Point / 2 / numOfTc, 2) : Math.Round(candidate.Point / 2, 2);

                    //Count testcases true
                    int tcCount = 0;

                    //STARTING FOR GRADING
                    comment += "- Check Data: ";
                    if (CompareTwoDataSetsByExcept(dataTableSolution, dataTableAnswer))
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
                                comment += string.Concat("Passed => +", tcPoint, "\n");
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
                            //string queryCheckSortAnswer = "USE " + dbAnswerName + ";\nSELECT DISTINCT* FROM (" + answer + "\nOFFSET 0 ROWS) " + "[" + dbAnswerName + "]";
                            //string queryCheckSortSolution = "USE " + dbSolutionName + ";\nSELECT DISTINCT* FROM (" + candidate.Solution + "\nOFFSET 0 ROWS) " + "[" + dbSolutionName + "]";


                            //DataTable dataTableSortAnswer = new DataTable();
                            //DataTable dataTableSortSolution = new DataTable();
                            ////Running Answer Sort query
                            //try
                            //{
                            //    using (SqlCommand sqlCommandSortAnswer = new SqlCommand(queryCheckSortAnswer, connection))
                            //    {
                            //        dataTableSortAnswer.Load(sqlCommandSortAnswer.ExecuteReader());
                            //    }
                            //}
                            //catch (Exception e)
                            //{
                            //    throw e;
                            //}
                            ////Running Solution Sort query
                            //try
                            //{
                            //    using (SqlCommand sqlCommandSortSolution = new SqlCommand(queryCheckSortSolution, connection))
                            //    {
                            //        dataTableSortSolution.Load(sqlCommandSortSolution.ExecuteReader());
                            //    }
                            //}
                            //catch (Exception e)
                            //{
                            //    throw e;
                            //}

                            //Compare number of rows
                            if (CompareTwoDataSetsByRow(dataTableAnswer, dataTableSolution))
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
                        comment += "Not pass => Point = 0\nStop checking\n";
                    }

                    //Calculate Point
                    if (numOfTc > 0 && numOfTc == tcCount)
                    {
                        gradePoint = candidate.Point;
                    }
                    else
                    {
                        gradePoint = Math.Round(tcCount * tcPoint + gradePoint, 2);
                    }
                    comment = string.Concat("Total Point: ", gradePoint, "/", candidate.Point, "\n", comment);
                    return new Dictionary<string, string>
                {
                    {"Point", gradePoint.ToString()},
                    {"Comment", comment}
                };
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        ///     Compare Multiple result set
        /// </summary>
        /// <param name="dbAnswerName">DB Name to check student query</param>
        /// <param name="dbSolutionName">DB Name to check teacher query</param>
        /// <param name="candidate">Candidate</param>
        /// <param name="errorMessage"></param>
        /// <exception cref="Exception"></exception>
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
                List<TestCase> testCases = GetTestCasesPoint(input: candidate.TestQuery);

                //Commnet
                string comment = errorMessage;

                double gradePoint = 0;
                double maxPoint = candidate.Point;

                // Prepare Command
                SqlConnectionStringBuilder builder = Constant.SqlConnectionStringBuilder;
                builder.MultipleActiveResultSets = true;
                string queryAnswer = "USE " + dbAnswerName + " \n" + candidate.TestQuery;
                string querySolution = "USE " + dbSolutionName + " \n" + candidate.TestQuery;
                // Connect and run query to check
                using (SqlConnection connection = new SqlConnection(connectionString: builder.ConnectionString))
                {
                    connection.Open();

                    // Prepare SqlDataAdapter
                    using (SqlDataAdapter sqlDataAdapterAnswer = new SqlDataAdapter(selectCommandText: queryAnswer, selectConnection: connection))
                    using (SqlDataAdapter sqlDataAdapterSolution = new SqlDataAdapter(selectCommandText: querySolution, selectConnection: connection))
                    {
                        // Prepare DataSet  
                        DataSet dataSetAnswer = new DataSet();
                        DataSet dataSetSolution = new DataSet();

                        // Fill Data adapter to dataset
                        sqlDataAdapterAnswer.Fill(dataSet: dataSetAnswer);
                        sqlDataAdapterSolution.Fill(dataSet: dataSetSolution);

                        if (dataSetSolution.Tables.Count != testCases.Count)
                        {
                            throw new Exception(
                                "Compare error: Number of testcases from comment and test query is difference");
                        }

                        //Compare results one by one
                        int countTesting = 0;
                        int countComparison = 0;

                        foreach (DataTable dataTableSolution in dataSetSolution.Tables)
                        {
                            countTesting++;
                            comment += string.Concat("TC ", countTesting, ": ",
                                testCases.ElementAt(index: countTesting - 1).Description, " - ");
                            foreach (DataTable dataTableAnswer in dataSetAnswer.Tables)
                            {
                                if (CompareTwoDataSetsByRow(dataTableAnswer, dataTableSolution))
                                {
                                    gradePoint += testCases.ElementAt(index: countTesting - 1).Point;
                                    comment += string.Concat(arg0: "Passed => +", arg1: testCases.ElementAt(index: countTesting - 1).Point,
                                        arg2: "\n");
                                    break;
                                }
                                countComparison++;
                            }
                            if (countComparison == dataSetAnswer.Tables.Count)
                            {
                                comment += "Not pass\n";
                            }
                            countComparison = 0;
                        }

                        //Degree 50% of point if Answer has more resultSets than Solution
                        if (dataSetSolution.Tables.Count < dataSetAnswer.Tables.Count)
                        {
                            double rate = (double) dataSetSolution.Tables.Count / dataSetAnswer.Tables.Count;
                            double rateFormatted = Math.Round(rate, 2);
                            comment = string.Concat(comment,
                                "Decrease Max Point by ", rateFormatted * 100, "% because Answer has more resultSets than Solution (",
                                dataSetAnswer.Tables.Count, " > ", dataSetSolution.Tables.Count, ")\n");
                            gradePoint *= rateFormatted;
                        }
                        comment = string.Concat("Total Point: ", gradePoint, "/", candidate.Point, "\n", comment);
                        gradePoint = gradePoint > maxPoint ? maxPoint : gradePoint;
                        return new Dictionary<string, string>
                        {
                            {"Point", gradePoint.ToString()},
                            {"Comment", comment}
                        };
                    }
                }
            }
            catch (SqlException e)
            {
                throw e;
            }

        }

        /// <summary>
        /// Check data using except 2-way (Rows can be difference)
        /// </summary>
        /// <param name="dataTableAnswer"></param>
        /// <param name="dataTableSolution"></param>
        /// <returns>
        /// true = same
        /// </returns>
        private static bool CompareTwoDataSetsByExcept(DataTable dataTableAnswer, DataTable dataTableSolution)
        {
            return !dataTableAnswer.AsEnumerable().Except(dataTableSolution.AsEnumerable(), DataRowComparer.Default).Any() && !dataTableSolution.AsEnumerable().Except(dataTableAnswer.AsEnumerable(), DataRowComparer.Default).Any();
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
                object[] rowArraySolution = dataTableSolution.Rows[i].ItemArray;
                object[] rowArrayAnswer = dataTableAnswer.Rows[i].ItemArray;
                if (!rowArraySolution.SequenceEqual(rowArrayAnswer))
                    return false;
            }
            return true;
        }
    }
}
