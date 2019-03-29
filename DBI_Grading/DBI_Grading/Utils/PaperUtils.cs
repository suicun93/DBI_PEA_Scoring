using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DBI_Grading.Common;
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
        internal static Dictionary<string, string> DmlType(Candidate candidate, string studentId, string answer,
            int questionOrder)
        {
            var dbSolutionName = studentId.Replace(" ", "") + questionOrder + "_Solution" + "_" +
                                 new Random().Next(10000000);
            var dbAnswerName = studentId.Replace(" ", "") + questionOrder + "_Answer" + "_" +
                               new Random().Next(10000000);


            //Generate 2 new DB for student's answer and solution

            //Create db from solution at schema question
            string queryDbForDml;
            if (Constant.PaperSet.DBScriptList.Count > 1)
                queryDbForDml = Constant.PaperSet.DBScriptList[1];
            else
                queryDbForDml = Constant.PaperSet.DBScriptList[0];
            if (candidate.RelatedSchema)
            {
                queryDbForDml = "";
                foreach (var question in Constant.PaperSet.QuestionSet.QuestionList)
                    if (question.Candidates.ElementAt(0).QuestionType == Candidate.QuestionTypes.Schema)
                    {
                        foreach (var candi in question.Candidates)
                            queryDbForDml = string.Concat(queryDbForDml, "\n", candi.Solution);
                        break;
                    }
            }

            try
            {
                //Generate databases for solution and answer
                General.GenerateDatabase(dbSolutionName, dbAnswerName, queryDbForDml);

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
                }
                try
                {
                    General.ExecuteSingleQuery(candidate.Solution, dbSolutionName);
                }
                catch (Exception e)
                {
                    if (e.InnerException != null) throw new Exception(e.InnerException.Message);
                    throw new Exception("Compare error: " + e.Message);
                }
                return ThreadUtils.WithTimeout(
                    () => CompareUtils.CompareOthersType(dbAnswerName, dbSolutionName, candidate, errorMessage),
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
                    () => CompareUtils.CompareOthersType(dbAnswerName, dbSolutionName, candidate, errorMessage),
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