using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DBI_Grading.Common;
using DBI_Grading.Model.Student;
using DBI_Grading.Model.Teacher;
using DBI_Grading.Utils.Dao;

namespace DBI_Grading.Utils
{
    public class PaperUtils
    {
        /// <summary>
        ///     Test Schema of 2 DBs.
        /// </summary>
        /// <param name="candidate">Question and requirement to check</param>
        /// <param name="studentId"></param>
        /// <param name="answer">Answer of student</param>
        /// <param name="questionOrder"></param>
        /// <returns>Result when compare 2 DB</returns>
        /// <exception cref="SqlException">
        ///     When something's wrong, throw exception to log error to KhaoThi
        /// </exception>
        internal static Dictionary<string, string> SchemaType(Candidate candidate, string studentId, string answer,
            int questionOrder)
        {
            var dbSolutionName = studentId.Replace(" ", "") + questionOrder + "_Solution" + "_" +
                                 new Random().Next(1000000000);
            var dbAnswerName = studentId.Replace(" ", "") + "_" + questionOrder + "_Answer" + "_" +
                               new Random().Next(1000000000);
            var dbEmptyName = studentId.Replace(" ", "") + questionOrder + "_EmptyDb" + "_" +
                              new Random().Next(1000000000);
            var querySolution = string.Concat("create database [", dbSolutionName, "]\nGO\nUSE [", dbSolutionName,
                "]\n", candidate.Solution);
            var queryAnswer = string.Concat("create database [", dbAnswerName, "]\nGO\nUSE [", dbAnswerName, "]\n",
                answer);
            var queryEmptyDb = string.Concat("create database [", dbEmptyName, "]");

            try
            {
                var errorMessage = "";
                // Execute query
                try
                {
                    General.ExecuteSingleQuery(queryAnswer, "master");
                }
                catch (Exception e)
                {
                    //Keep grading instead of errors
                    if (e.InnerException != null)
                        errorMessage = string.Concat("Answer query error: ", e.InnerException.Message, "\n");
                    else
                        errorMessage = string.Concat("Answer query error: ", e.Message, "\n");
                }
                try
                {
                    General.ExecuteSingleQuery(querySolution, "master");
                    General.ExecuteSingleQuery(queryEmptyDb, "master");
                }
                catch (Exception e)
                {
                    throw new Exception("Compare error: " + e.Message);
                }
                // Execute query
                return ThreadUtils.WithTimeout(
                    () => CompareUtils.CompareSchemaType(dbAnswerName, dbSolutionName, dbEmptyName, candidate, errorMessage),
                    Constant.TimeOutInSecond);
            }
            finally
            {
                General.KillAllSessionSql();
                General.DropDatabase(dbAnswerName);
                General.DropDatabase(dbSolutionName);
                General.DropDatabase(dbEmptyName);
            }
        }

        /// <summary>
        ///     Test Simple Query
        /// </summary>
        /// <param name="candidate">Question and requirement to check</param>
        /// <param name="studentId"></param>
        /// <param name="answer">Answer of student</param>
        /// <param name="questionOrder"></param>
        /// <returns>Result when compare 2 table</returns>
        /// <exception cref="Exception">
        ///     When something's wrong, throw exception to log error to KhaoThi
        /// </exception>
        internal static Dictionary<string, string> SelectType(Candidate candidate, string studentId, string answer,
            int questionOrder)
        {
            var dbSolutionName = studentId.Replace(" ", "") + questionOrder + "_Solution" + "_" +
                                 new Random().Next(1000000000);
            var dbAnswerName = studentId.Replace(" ", "") + questionOrder + "_Answer" + "_" +
                               new Random().Next(1000000000);

            try
            {
                //Generate 2 new DB for student's answer and solution
                if (Constant.PaperSet.DBScriptList.Count > 1)
                    General.GenerateDatabase(dbSolutionName, dbAnswerName, Constant.PaperSet.DBScriptList[1]);
                else
                    General.GenerateDatabase(dbSolutionName, dbAnswerName, Constant.PaperSet.DBScriptList[0]);
                //Compare
                return ThreadUtils.WithTimeout(
                    () => CompareUtils.CompareSelectType(dbAnswerName, dbSolutionName, answer, candidate),
                    Constant.TimeOutInSecond);
            }
            finally
            {
                General.KillAllSessionSql();
                General.DropDatabase(dbSolutionName);
                General.DropDatabase(dbAnswerName);
            }
        }

        /// <summary>
        ///     Execute Query and compare 2 effected tables
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="studentId"></param>
        /// <param name="answer">Answer of student</param>
        /// <param name="questionOrder"></param>
        /// <returns>Result when compare 2 result tables</returns>
        /// <exception cref="Exception">
        ///     When something's wrong, throw exception to log to Khao Thi.
        /// </exception>
        internal static Dictionary<string, string> DmlType(Candidate candidate, string studentId, string answer, int questionOrder, List<Candidate> candidates, string schemaAnswer)
        {
            var dbSolutionName = studentId.Replace(" ", "") + questionOrder + "_Solution" + "_" +
                                 new Random().Next(10000000);
            var dbAnswerName = studentId.Replace(" ", "") + questionOrder + "_Answer" + "_" +
                               new Random().Next(10000000);


            //Create db from solution at schema question

            var message = "";
            Dictionary<string, string> resultCase1 = new Dictionary<string, string>();
            Dictionary<string, string> resultCase2 = new Dictionary<string, string>();

            //Case 1: Use Schema's Solution
            string queryDbForDml = "";
            string solutionQuery = "";
            string testQuery = "";
            if (Constant.PaperSet.DBScriptList.Count > 1)
                queryDbForDml = Constant.PaperSet.DBScriptList[1];
            else
                queryDbForDml = Constant.PaperSet.DBScriptList[0];
            if (candidate.RelatedSchema)
            {
                queryDbForDml = "";
                foreach (var question in Constant.PaperSet.QuestionSet.QuestionList)
                {
                    if (question.Candidates.ElementAt(0).QuestionType.Equals(Candidate.QuestionTypes.Schema))
                    {
                        foreach (var candi in question.Candidates)
                            queryDbForDml = string.Concat(queryDbForDml, "\n", candi.Solution);
                    }
                    if (question.Candidates.ElementAt(0).QuestionType.Equals(Candidate.QuestionTypes.DML))
                    {
                        foreach (var candi in question.Candidates)
                        {
                            solutionQuery = string.Concat(solutionQuery, "\n", candi.Solution);
                            testQuery = string.Concat(testQuery, "\n", candi.TestQuery);
                        }
                    }
                }


            }

            try
            {
                //Generate databases for solution and answer
                General.GenerateDatabase(dbSolutionName, dbAnswerName, queryDbForDml);

                // Execute query
                try
                {
                    General.ExecuteSingleQuery(answer, dbAnswerName);
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                        message += string.Concat("Answer query error: ", e.InnerException.Message, "\n");
                    else message += string.Concat("Answer query error: " + e.Message, "\n");
                }
                try
                {
                    General.ExecuteSingleQuery(solutionQuery, dbSolutionName);
                }
                catch (Exception e)
                {
                    if (e.InnerException != null) throw new Exception(e.InnerException.Message);
                    throw new Exception("Compare error: " + e.Message);
                }
                resultCase1 = ThreadUtils.WithTimeout(() => CompareUtils.CompareDmlType(dbAnswerName, dbSolutionName, candidate, testQuery, message),
                    Constant.TimeOutInSecond);
            }
            finally
            {
                General.KillAllSessionSql();
                General.DropDatabase(dbSolutionName);
                General.DropDatabase(dbAnswerName);
            }

            //case 2: Use student's schema
            dbAnswerName = studentId.Replace(" ", "") + questionOrder + "_AnswerSchema" + "_" +
                               new Random().Next(10000000);
            dbSolutionName = studentId.Replace(" ", "") + questionOrder + "_SolutionSchema" + "_" +
                             new Random().Next(1000000000);
            try
            {

                if (string.IsNullOrEmpty(schemaAnswer.Trim()))
                {
                    return resultCase1;
                }

                try
                {
                    if (string.IsNullOrEmpty(schemaAnswer.Trim()))
                        return resultCase1;

                    var queryGenerateAnswerDb = "CREATE DATABASE [" + dbAnswerName + "]\n" +
                                                "GO\n" +
                                                "USE " + "[" + dbAnswerName + "]\n" + schemaAnswer;
                    General.ExecuteSingleQuery(queryGenerateAnswerDb, "master");

                    queryDbForDml = "CREATE DATABASE [" + dbSolutionName + "]\n" +
                                                  "GO\n" +
                                                  "USE " + "[" + dbSolutionName + "]\n" + queryDbForDml;
                    General.ExecuteSingleQuery(queryDbForDml, "master");
                }
                catch
                {
                    return resultCase1;
                }

                // Execute query
                try
                {
                    General.ExecuteSingleQuery(answer, dbAnswerName);
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                        message += string.Concat("Answer query error: ", e.InnerException.Message, "\n");
                    else message += string.Concat("Answer query error: " + e.Message, "\n");
                }
                try
                {
                    General.ExecuteSingleQuery(solutionQuery, dbSolutionName);
                }
                catch
                {
                    //ignored
                }
                resultCase2 = ThreadUtils.WithTimeout(() => CompareUtils.CompareDmlType(dbAnswerName, dbSolutionName, candidate, testQuery, message),
                    Constant.TimeOutInSecond);
            }
            finally
            {
                General.KillAllSessionSql();
                General.DropDatabase(dbSolutionName);
                General.DropDatabase(dbAnswerName);
            }
            if (resultCase2 != null && !string.IsNullOrEmpty(resultCase2["Point"]) && double.Parse(resultCase2["Point"]) > double.Parse(resultCase1["Point"]))
                 return resultCase2;

            return resultCase1;
        }

        /// <summary>
        ///     Execute Query and compare 2 effected tables
        /// </summary>
        /// <param name="candidate">Question and requirement to check</param>
        /// <param name="studentId"></param>
        /// <param name="answer">Answer of student</param>
        /// <param name="questionOrder"></param>
        /// <returns>Result when compare 2 result tables</returns>
        /// <exception cref="Exception">
        ///     When something's wrong, throw exception to log to Khao Thi.
        /// </exception>
        internal static Dictionary<string, string> TriggerProcedureType(Candidate candidate, string studentId,
            string answer, int questionOrder)
        {
            var dbSolutionName = studentId.Replace(" ", "") + questionOrder + "_Solution" + "_" +
                                 new Random().Next(10000000);
            var dbAnswerName = studentId.Replace(" ", "") + questionOrder + "_Answer" + "_" +
                               new Random().Next(10000000);

            //Generate 2 new DB for student's answer and solution
            if (Constant.PaperSet.DBScriptList.Count > 1)
                General.GenerateDatabase(dbSolutionName, dbAnswerName, Constant.PaperSet.DBScriptList[1]);
            else
                General.GenerateDatabase(dbSolutionName, dbAnswerName, Constant.PaperSet.DBScriptList[0]);
            try
            {
                var errorMessage = "";
                // Execute query
                try
                {
                    General.ExecuteSingleQuery(answer, dbAnswerName);
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                        errorMessage += string.Concat("Answer query error: ", e.InnerException.Message, "\n");
                    else errorMessage += string.Concat("Answer query error: " + e.Message, "\n");
                    //Still grading for student even error
                    //Student still right at some testcase, need to keep grading
                }
                try
                {
                    General.ExecuteSingleQuery(candidate.Solution, dbSolutionName);
                }
                catch (Exception e)
                {
                    if (e.InnerException != null) throw new Exception("Compare error: " + e.InnerException.Message);
                    throw new Exception("Compare error: " + e.Message);
                }
                return ThreadUtils.WithTimeout(
                    () => CompareUtils.CompareSpAndTrigger(dbAnswerName, dbSolutionName, candidate, errorMessage),
                    Constant.TimeOutInSecond);
            }
            finally
            {
                General.KillAllSessionSql();
                General.DropDatabase(dbSolutionName);
                General.DropDatabase(dbAnswerName);
            }
        }
    }
}