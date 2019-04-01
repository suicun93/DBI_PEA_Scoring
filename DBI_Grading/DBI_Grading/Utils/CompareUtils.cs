using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DBI_Grading.Model.Teacher;
using DBI_Grading.Utils.Dao;

namespace DBI_Grading.Utils
{
    internal class CompareUtils
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
        public static Dictionary<string, string> CompareSchemaType(string dbAnswerName, string dbSolutionName,
            string dbEmptyName, Candidate candidate, string errorMessage)
        {
            //Prepare query
            var compareQuery = "exec sp_CompareDb [" + dbSolutionName + "], [" + dbAnswerName + "]";
            var countComparisonQuery = "exec sp_CompareDb [" + dbSolutionName + "], [" + dbEmptyName + "]";
            //Comment result
            var comment = errorMessage;

            //1. Count tables
            var countAnswerTables = General.GetNumberOfTablesInDatabase(dbAnswerName);
            var countSolutionTables = General.GetNumberOfTablesInDatabase(dbSolutionName);

            if (countAnswerTables <= 0)
                return new Dictionary<string, string>
                {
                    {"Point", "0"},
                    {"Comment", comment}
                };

            //Decrease maxpoint by rate
            //Max point
            var maxPoint = candidate.Point;
            comment += "Count Tables in database: ";
            if (countAnswerTables > countSolutionTables)
            {
                var ratePoint = (double)countSolutionTables / countAnswerTables;
                maxPoint = Math.Round(candidate.Point * ratePoint, 4);
                comment += string.Concat("Answer has more tables than Solution's database (", countAnswerTables, ">",
                    countSolutionTables, ") => Decrease Max Point by ", Math.Round(ratePoint * 100, 4),
                    "% => Max Point = ", maxPoint,
                    "\n");
            }
            else if (countSolutionTables > countAnswerTables)
            {
                comment += string.Concat("Answer has less tables than Solution's database (", countAnswerTables, "<",
                    countSolutionTables, ")\n");
            }
            else
            {
                comment += "Same\n";
            }
            //Count Comparison
            int numOfComparison;
            using (var dtsNumOfComparison = General.GetDataSetFromReader(countComparisonQuery))
            {
                numOfComparison = dtsNumOfComparison.Tables[0].Rows.Count * 2 + dtsNumOfComparison.Tables[1].Rows.Count;
            }
            //Get Dataset compare result
            using (var dtsCompare = General.GetDataSetFromReader(compareQuery))
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
                var gradePoint = Math.Round(maxPoint * (numOfComparison - totalErros) / numOfComparison, 4);
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
                if (gradePoint > maxPoint) gradePoint = maxPoint;
                return new Dictionary<string, string>
                {
                    {"Point", gradePoint.ToString()},
                    {"Comment", comment}
                };
            }
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
        public static Dictionary<string, string> CompareSelectType(string dbAnswerName, string dbSolutionName,
            string answer, Candidate candidate)
        {
            //Running answer query// index 0 as data table, index 1 as schema table
            var dataTableAnswer = General.GetDataTableFromReader("USE [" + dbAnswerName + "];\n" + answer + "");

            //Running Tq 
            var dataTableTq = General.GetDataTableFromReader("USE [" + dbSolutionName + "];\n" + candidate.TestQuery + "");

            //Running Tq 
            var dataTableSolution = General.GetDataTableFromReader("USE [" + dbSolutionName + "];\n" + candidate.Solution + "");

            //Number of testcases
            var numOfTc = 0;
            if (candidate.RequireSort) numOfTc++;
            if (candidate.CheckColumnName) numOfTc++;
            if (candidate.CheckDistinct) numOfTc++;

            //Prepare point for testcases and data
            var dataPoint = numOfTc != 0 ? Math.Round(candidate.Point / 2, 4) : candidate.Point;

            double gradePoint = 0; //Grading Point
            var comment = ""; //Logs

            //Point for each testcase passed
            var tcPoint = numOfTc > 0
                ? Math.Round(candidate.Point / 2 / numOfTc, 4)
                : Math.Round(candidate.Point / 2, 4);

            //Count testcases true
            var tcCount = 0;

            //Format column name
            General.LowerCaseColumnName(dataTableAnswer);
            General.LowerCaseColumnName(dataTableSolution);
            General.LowerCaseColumnName(dataTableAnswer);

            //STARTING FOR GRADING
            comment += "- Check Data: ";
            try
            {
                if (General.CompareData(dataTableAnswer.Copy(), dataTableTq.Copy(), false))
                {
                    gradePoint += dataPoint;
                    comment += string.Concat("Passed => +", dataPoint, "\n");

                    //1. Check if distinct is required
                    if (candidate.CheckDistinct)
                    {
                        comment += "- Check distinct: ";
                        //Compare number of rows
                        if (dataTableTq.Rows.Count == dataTableAnswer.Rows.Count)
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
                        //Compare row by row
                        if (General.CompareTwoDataTablesByRow(dataTableAnswer.Copy(), dataTableSolution.Copy()))
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
                        var resCompareColumnName = General.CompareColumnsName(dataTableAnswer, dataTableTq);
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
            }
            catch (Exception e)
            {
                throw new Exception(comment + e.Message + " => Point = 0");
            }


            //Calculate Point
            if (numOfTc > 0 && numOfTc == tcCount)
                gradePoint = candidate.Point;
            else
                gradePoint = Math.Round(tcCount * tcPoint + gradePoint, 4);
            if (gradePoint > candidate.Point) gradePoint = Math.Round(candidate.Point, 4);
            comment = string.Concat("Total Point: ", gradePoint, "/", candidate.Point, "\n", comment);
            return new Dictionary<string, string>
            {
                {"Point", gradePoint.ToString()},
                {"Comment", comment}
            };
        }

        /// <summary>
        ///     SP/Trigger Type
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
        public static Dictionary<string, string> CompareSpAndTrigger(string dbAnswerName, string dbSolutionName,
            Candidate candidate, string errorMessage)
        {
            //Get testcases from comment in test query
            var testCases = StringUtils.GetTestCases(candidate.TestQuery, candidate);

            //Commnet
            var comment = errorMessage;
            var countTesting = 0;
            var countTrueTc = 0;

            double gradePoint = 0;
            var maxPoint = candidate.Point;
            try
            {
                foreach (var testCase in testCases)
                {
                    comment += string.Concat("TC ", ++countTesting, ": ",
                        testCase.Description, "- ");
                    // Prepare query
                    var queryAnswer = "USE [" + dbAnswerName + "] \n" + testCase.TestQuery;
                    var querySolution = "USE [" + dbSolutionName + "] \n" + testCase.TestQuery;

                    // Prepare DataSet  
                    var dataSetAnswer = General.GetDataSetFromReader(queryAnswer);
                    var dataSetSolution = General.GetDataSetFromReader(querySolution);

                    if (General.CompareTwoDataSets(dataSetAnswer, dataSetSolution, false))
                    {
                        var decreaseRate = (double)dataSetSolution.Tables.Count / dataSetAnswer.Tables.Count;
                        var maxTcPoint = Math.Round(testCase.RatePoint * decreaseRate * candidate.Point, 4);
                        if (dataSetAnswer.Tables.Count > dataSetSolution.Tables.Count)
                        {
                            comment += string.Concat("Answer has more tables than Solution's database (",
                                dataSetAnswer.Tables.Count, ">",
                                dataSetSolution.Tables.Count, ") => Decrease Max Point by ", Math.Round(decreaseRate * 100, 4),
                                "% => Max TC Point = ", maxTcPoint,
                                "\n");
                            countTesting--;
                        }
                        gradePoint += maxTcPoint;
                        comment += string.Concat("Passed => +", maxTcPoint, "\n");
                        countTrueTc++;
                    }
                    else
                    {
                        comment += "Not pass\n";
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(comment + e.Message);
            }

            comment = string.Concat("Total Point: ", gradePoint, "/", candidate.Point, "\n", comment);
            gradePoint = countTrueTc == testCases.Count ? maxPoint : gradePoint;
            if (gradePoint > maxPoint) gradePoint = maxPoint;
            return new Dictionary<string, string>
            {
                {"Point", gradePoint.ToString()},
                {"Comment", comment}
            };
        }

        /// <summary>
        ///     Insert/Update/Delete Type
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
        public static Dictionary<string, string> CompareDmlType(string dbAnswerName, string dbSolutionName, Candidate candidate, string testQuery, string errorMessage)
        {
            //Get testcases from comment in test query
            var testCases = StringUtils.GetTestCases(testQuery, candidate);

            //Commnet
            var comment = errorMessage;
            var countTesting = 0;
            var countTrueTc = 0;

            double gradePoint = 0;
            var maxPoint = candidate.Point;

            foreach (var testCase in testCases)
            {
                try
                {
                    if (countTesting == 0)
                    {
                        comment += "======= Grading case 1 =======";
                    }
                    else if (countTesting == testCases.Count / 2)
                    {
                        comment += "======= Grading case 2 =======";
                    }
                    comment += string.Concat("TC ", ++countTesting, ": ",
                        testCase.Description, "- ");
                    // Prepare query
                    var queryAnswer = "USE [" + dbAnswerName + "] \n" + testCase.TestQuery;
                    var querySolution = "USE [" + dbSolutionName + "] \n" + testCase.TestQuery;

                    // Prepare DataSet  
                    var dataSetAnswer = General.GetDataSetFromReader(queryAnswer);
                    var dataSetSolution = General.GetDataSetFromReader(querySolution);

                    if (General.CompareTwoDataSets(dataSetAnswer, dataSetSolution, true))
                    {
                        var tcPoint = Math.Round(testCase.RatePoint * maxPoint, 4);
                        gradePoint += tcPoint;
                        comment += string.Concat("Passed => +", tcPoint, "\n");
                        countTrueTc++;
                    }
                    else if (General.CompareTwoDataSets(dataSetAnswer, dataSetSolution, false))
                    {
                        var tcPoint = Math.Round(testCase.RatePoint * maxPoint, 4);
                        gradePoint += tcPoint;
                        comment += string.Concat("Passed => +", tcPoint, "\n");
                        countTrueTc++;
                    }
                    else
                    {
                        comment += "Skip/Not Pass\n";
                    }
                }
                catch
                {
                    comment = comment + "Skip/Not Pass\n";
                }
            }
            comment = string.Concat("Total Point: ", gradePoint, "/", candidate.Point, "\n", comment);
            gradePoint = countTrueTc == testCases.Count ? maxPoint : gradePoint;
            if (gradePoint > maxPoint) gradePoint = maxPoint;
            return new Dictionary<string, string>
            {
                {"Point", gradePoint.ToString()},
                {"Comment", comment}
            };
        }
    }
}