using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DBI_PEA_Grading.Common;
using DBI_PEA_Grading.Model.Teacher;

// ReSharper disable SpecifyACultureInStringConversionExplicitly

namespace DBI_PEA_Grading.Utils.Dao
{
    public partial class General
    {
        /// <summary>
        ///     Compare 2 databases
        /// </summary>
        /// <param name="dbAnswerName">Student Database Name</param>
        /// <param name="dbSolutionName">Solution Database Name</param>
        /// <param name="dbEmptyName"></param>
        /// <param name="candidate"></param>
        /// <param name="errorMessage"></param>
        /// <returns>
        ///     "true" if correct
        ///     "false" if wrong
        ///     message error from sqlserver if error
        /// </returns>
        public static Dictionary<string, string> CompareTwoDatabases(string dbAnswerName, string dbSolutionName,
            string dbEmptyName, Candidate candidate, string errorMessage)
        {
            //Prepare query
            var compareQuery = "exec sp_CompareDb [" + dbSolutionName + "], [" + dbAnswerName + "]";
            var countComparisonQuery = "exec sp_CompareDb [" + dbSolutionName + "], [" + dbEmptyName + "]";
            //Comment result
            var comment = errorMessage;

            //1. Count tables
            var countAnswerTables = GetNumberOfTablesInDatabase(dbAnswerName);
            var countSolutionTables = GetNumberOfTablesInDatabase(dbSolutionName);

            if (countAnswerTables <= 0)
                return new Dictionary<string, string>
                {
                    {"Point", "0"},
                    {"Comment", comment}
                };

            //Decrease maxpoint by rate
            double ratePoint;
            //Max point
            var maxPoint = candidate.Point;
            comment += "Count Tables in database: ";
            if (countAnswerTables > countSolutionTables)
            {
                ratePoint = (double)countSolutionTables / countAnswerTables;
                maxPoint = Math.Round(candidate.Point * ratePoint, 2);
                comment += string.Concat("Answer has more columns than Solution database (", countAnswerTables, ">",
                    countSolutionTables, ") => Decrease Max Point by ", ratePoint * 100, "% => Max Point = ", maxPoint,
                    "\n");
            }
            else if (countSolutionTables > countAnswerTables)
            {
                comment += string.Concat("Answer has less columns than Solution database (", countAnswerTables, "<",
                    countSolutionTables, ")\n");
            }
            else
            {
                comment += "Same\n";
            }
            //Count Comparison
            int numOfComparison;
            using (var dtsNumOfComparison = GetDataSetFromReader(countComparisonQuery))
            {
                numOfComparison = dtsNumOfComparison.Tables[0].Rows.Count * 2 + dtsNumOfComparison.Tables[1].Rows.Count;
            }
            //Get Dataset compare result
            using (var dtsCompare = GetDataSetFromReader(compareQuery))
            {
                if (dtsCompare == null)
                    throw new Exception("Compare error");
                //Get all errors
                var errorsConstructureRows = dtsCompare.Tables[0].AsEnumerable()
                    .Where(myRow => myRow.Field<string>("DATABASENAME").Contains("Solution"));
                var errorsConstraintRows = dtsCompare.Tables[1].AsEnumerable()
                    .Where(myRow => myRow.Field<string>("DATABASENAME").Contains("Solution"));
                //Sumary point
                var totalErros = errorsConstraintRows.Count() + errorsConstructureRows.Count();
                var gradePoint = Math.Round(maxPoint * (numOfComparison - totalErros) / numOfComparison, 2);
                comment += string.Concat("Correct ", numOfComparison - totalErros, "/", numOfComparison,
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
                        myRow.Field<string>("REASON").Equals("Missing column or Wrong column name") &&
                        myRow.Field<string>("DATABASENAME").Contains("Solution"));
                    if (defErrors.Any())
                    {
                        comment += "+ Definition not matching:\n";
                        foreach (var rowSolution in defErrors)
                        {
                            comment += string.Concat("    Required: ", rowSolution["TABLENAME"], "(",
                                rowSolution["COLUMNNAME"], ") => ", rowSolution["DATATYPE"], ", ",
                                rowSolution["NULLABLE"], "\n");
                            var rowAnswer = dtsCompare.Tables[0].AsEnumerable().Where(myRow =>
                                myRow.Field<string>("REASON").Equals("Definition not matching") &&
                                myRow.Field<string>("COLUMNNAME").ToLower()
                                    .Equals(rowSolution["COLUMNNAME"].ToString().ToLower())
                                && myRow.Field<string>("TABLENAME").ToLower()
                                    .Equals(rowSolution["TABLENAME"].ToString().ToLower()) &&
                                myRow.Field<string>("DATABASENAME").Contains("Answer")).ElementAt(0);
                            comment += string.Concat("    Answer  : ", rowAnswer["TABLENAME"], "(",
                                rowAnswer["COLUMNNAME"], ") => ", rowAnswer["DATATYPE"], ", ", rowAnswer["NULLABLE"],
                                "\n");
                        }
                    }
                    if (missingErrors.Any())
                    {
                        comment += "+ Column missing: ";
                        foreach (var rowSolution in missingErrors)
                            comment += string.Concat(rowSolution["COLUMNNAME"], "(", rowSolution["TABLENAME"], "), ");
                        comment = string.Concat(comment.Remove(comment.Length - 2), "\n");
                    }
                }
                //About Constraints
                if (errorsConstraintRows.Any())
                {
                    comment += "- Constraints check:\n";
                    foreach (var rowSolution in errorsConstraintRows)
                        comment += string.Concat("  Missing ", rowSolution["PK_COLUMNS"], "(", rowSolution["PK_TABLE"],
                            ") - ", rowSolution["FK_COLUMNS"], "(", rowSolution["FK_TABLE"], ")\n");
                }
                return new Dictionary<string, string>
                {
                    {"Point", gradePoint.ToString()},
                    {"Comment", comment}
                };
            }
        }

        /// <summary>
        ///     Compare Columns Name of tables
        /// </summary>
        /// <param name="dataTableAnswerSchema"></param>
        /// <param name="dataTableSolutionSchema"></param>
        /// <returns>"(Empty)" if true, "(comment)" if false</returns>
        public static string CompareColumnsNameOfTables(DataTable dataTableAnswerSchema,
            DataTable dataTableSolutionSchema)
        {
            for (var i = 0; i < dataTableSolutionSchema.Rows.Count; i++)
                if (!dataTableSolutionSchema.Rows[i]["ColumnName"].ToString().ToLower()
                    .Equals(dataTableAnswerSchema.Rows[i]["ColumnName"].ToString().ToLower()))
                    return "Column Name wrong - " + dataTableSolutionSchema.Rows[i]["ColumnName"];
            return "";
        }

        /// <summary>
        ///     Compare tables with sort
        /// </summary>
        /// <param name="dbAnswerName"></param>
        /// <param name="dbSolutionName"></param>
        /// <param name="answer"></param>
        /// <param name="candidate"></param>
        /// <returns>
        ///     "true" if correct
        ///     "false" if wrong
        ///     message error from sqlserver if error
        /// </returns>
        public static Dictionary<string, string> CompareOneResultSet(string dbAnswerName, string dbSolutionName,
            string answer,
            Candidate candidate)
        {
            var builder = Constant.SqlConnectionStringBuilder;
            var dataTableAnswer = new DataTable();
            var dataTableSolution = new DataTable();
            var dataTableAnswerShema = new DataTable();
            var dataTableSolutionShema = new DataTable();
            Dictionary<string, string> r;
            // Connect to SQL
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                SqlCommand sqlCommandAnswer = null;
                SqlDataReader sqlReaderAnswer = null;
                SqlCommand sqlCommandSolution = null;
                SqlDataReader sqlReaderSolution = null;

                r = Utilities.WithTimeout<Dictionary<string, string>>(proc: () =>
                  {
                      //Running answer query
                      sqlCommandAnswer = new SqlCommand("USE [" + dbAnswerName + "];\n" + answer + "", connection)
                      {
                          CommandTimeout = Constant.TimeOutInSecond
                      };
                      sqlReaderAnswer = sqlCommandAnswer.ExecuteReader();
                      if (candidate.CheckColumnName) dataTableAnswerShema = sqlReaderAnswer.GetSchemaTable();
                      dataTableAnswer.Load(sqlReaderAnswer);


                      //Running Solution 
                      sqlCommandSolution =
                          new SqlCommand("USE [" + dbSolutionName + "];\n" + candidate.Solution + "", connection)
                          {
                              CommandTimeout = Constant.TimeOutInSecond
                          };
                      sqlReaderSolution = sqlCommandSolution.ExecuteReader();
                      if (candidate.CheckColumnName) dataTableSolutionShema = sqlReaderSolution.GetSchemaTable();
                      dataTableSolution.Load(sqlReaderSolution);


                      //Number of testcases
                      var numOfTc = 0;
                      if (candidate.RequireSort) numOfTc++;
                      if (candidate.CheckColumnName) numOfTc++;
                      if (candidate.CheckDistinct) numOfTc++;

                      //Prepare point for testcases and data
                      var dataPoint = numOfTc != 0 ? Math.Round(candidate.Point / 2, 2) : candidate.Point;

                      double gradePoint = 0; //Grading Point
                      var comment = ""; //Logs

                      //Point for each testcase passed
                      var tcPoint = numOfTc > 0
                           ? Math.Round(candidate.Point / 2 / numOfTc, 2)
                           : Math.Round(candidate.Point / 2, 2);

                      //Count testcases true
                      var tcCount = 0;

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
                              var queryCheckSortAnswer = "USE [" + dbAnswerName + "];\nSELECT DISTINCT* FROM (" + answer +
                                                          "\nOFFSET 0 ROWS) " + "[" + dbAnswerName + "]";
                              var queryCheckSortSolution = "USE [" + dbSolutionName + "];\nSELECT DISTINCT* FROM (" +
                                                           candidate.Solution + "\nOFFSET 0 ROWS) " + "[" + dbSolutionName +
                                                           "]";

                              var dataTableSortAnswer = new DataTable();
                              var dataTableSortSolution = new DataTable();
                              //Running Answer Sort query
                              try
                              {
                                  using (var sqlCommandSortAnswer = new SqlCommand(queryCheckSortAnswer, connection))
                                  {
                                      dataTableSortAnswer.Load(sqlCommandSortAnswer.ExecuteReader());
                                  }
                                  using (var sqlCommandSortSolution = new SqlCommand(queryCheckSortSolution, connection))
                                  {
                                      dataTableSortSolution.Load(sqlCommandSortSolution.ExecuteReader());
                                  }
                              }
                              catch
                              {
                                  try
                                  {
                                      queryCheckSortAnswer =
                                          "USE [" + dbAnswerName + "];\nSELECT DISTINCT* FROM (" + answer + "\n) " + "[" +
                                          dbAnswerName + "]";
                                      queryCheckSortSolution =
                                          "USE [" + dbSolutionName + "];\nSELECT DISTINCT* FROM (" + candidate.Solution +
                                          "\n) " + "[" + dbSolutionName + "]";

                                      using (var sqlCommandSortAnswer = new SqlCommand(queryCheckSortAnswer, connection))
                                      {
                                          dataTableSortAnswer.Load(sqlCommandSortAnswer.ExecuteReader());
                                      }
                                      using (var sqlCommandSortSolution = new SqlCommand(queryCheckSortSolution, connection))
                                      {
                                          dataTableSortSolution.Load(sqlCommandSortSolution.ExecuteReader());
                                      }
                                  }
                                  catch
                                  {
                                      dataTableSortAnswer = dataTableAnswer;
                                      dataTableSortSolution = dataTableSolution;
                                  }
                              }

                              //Compare number of rows
                              if (CompareTwoDataSetsByRow(dataTableSortAnswer, dataTableSortSolution))
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
                              var resCompareColumnName =
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
                          gradePoint = candidate.Point;
                      else
                          gradePoint = Math.Round(tcCount * tcPoint + gradePoint, 2);
                      comment = string.Concat("Total Point: ", gradePoint, "/", candidate.Point, "\n", comment);
                      return new Dictionary<string, string>
                  {
                    {"Point", gradePoint.ToString()},
                    {"Comment", comment}
                  };
                  },
                  duration: Constant.TimeOutInSecond,
                  errorAction: (error) =>
                  {
                      try
                      {
                          try
                          {
                              sqlCommandAnswer?.Cancel(); //execute before closing the reader
                              sqlReaderAnswer?.Close();
                          }
                          catch (Exception)
                          {
                          }
                          try
                          {
                              sqlCommandSolution?.Cancel(); //execute before closing the reader
                              sqlReaderSolution?.Close();
                          }
                          catch (Exception)
                          {
                          }
                          connection.Close();
                      }
                      catch (Exception e)
                      {
                          Console.WriteLine(e);
                      }
                      throw error;
                  });

            }
            return r;
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
        ///     "true" if correct
        ///     "false" if wrong
        ///     message error from sqlserver if error
        /// </returns>
        public static Dictionary<string, string> CompareMoreResultSets(string dbAnswerName, string dbSolutionName,
            Candidate candidate, string errorMessage)
        {
            //Get testcases from comment in test query
            var testCases = GetTestCasesPoint(candidate);

            //Commnet
            var comment = errorMessage;

            double gradePoint = 0;
            var maxPoint = candidate.Point;

            // Prepare Command
            var builder = Constant.SqlConnectionStringBuilder;
            builder.MultipleActiveResultSets = true;
            var queryAnswer = "USE [" + dbAnswerName + "] \n" + candidate.TestQuery;
            var querySolution = "USE [" + dbSolutionName + "] \n" + candidate.TestQuery;
            // Connect and run query to check
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

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

                    if (dataSetSolution.Tables.Count != testCases.Count)
                        throw new Exception(
                            "Compare error: Number of testcases from comment and test query is difference");

                    //Compare results one by one
                    var countTesting = 0;
                    var countComparison = 0;

                    foreach (DataTable dataTableSolution in dataSetSolution.Tables)
                    {
                        countTesting++;
                        comment += string.Concat("TC ", countTesting, ": ",
                            testCases.ElementAt(countTesting - 1).Description, " - ");
                        foreach (DataTable dataTableAnswer in dataSetAnswer.Tables)
                        {
                            if (CompareTwoDataSetsByRow(dataTableAnswer, dataTableSolution))
                            {
                                gradePoint += testCases.ElementAt(countTesting - 1).Point;
                                comment += string.Concat("Passed => +", testCases.ElementAt(countTesting - 1).Point,
                                    "\n");
                                break;
                            }
                            countComparison++;
                        }
                        if (countComparison == dataSetAnswer.Tables.Count)
                            comment += "Not pass\n";
                        countComparison = 0;
                    }

                    //Degree 50% of point if Answer has more resultSets than Solution
                    if (dataSetSolution.Tables.Count < dataSetAnswer.Tables.Count)
                    {
                        var rate = (double)dataSetSolution.Tables.Count / dataSetAnswer.Tables.Count;
                        var rateFormatted = Math.Round(rate, 2);
                        comment = string.Concat(comment,
                            "Decrease Max Point by ", rateFormatted * 100,
                            "% because Answer has more resultSets than Solution (",
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

        /// <summary>
        ///     Check data using except 2-way (Rows can be difference)
        /// </summary>
        /// <param name="dataTableAnswer"></param>
        /// <param name="dataTableSolution"></param>
        /// <returns>
        ///     true = same
        /// </returns>
        private static bool CompareTwoDataSetsByExcept(DataTable dataTableAnswer, DataTable dataTableSolution)
        {
            return !dataTableAnswer.AsEnumerable().Except(dataTableSolution.AsEnumerable(), DataRowComparer.Default)
                       .Any() && !dataTableSolution.AsEnumerable()
                       .Except(dataTableAnswer.AsEnumerable(), DataRowComparer.Default).Any();
        }

        /// <summary>
        ///     Compare data line by row
        /// </summary>
        /// <param name="dataTableAnswer"></param>
        /// <param name="dataTableSolution"></param>
        /// <returns>
        ///     true = same
        /// </returns>
        private static bool CompareTwoDataSetsByRow(DataTable dataTableAnswer, DataTable dataTableSolution)
        {
            if (dataTableSolution.Rows.Count != dataTableAnswer.Rows.Count ||
                dataTableSolution.Columns.Count != dataTableAnswer.Columns.Count) return false;
            for (var i = 0; i < dataTableSolution.Rows.Count; i++)
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