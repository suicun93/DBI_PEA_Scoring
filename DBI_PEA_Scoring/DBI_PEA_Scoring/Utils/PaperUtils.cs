using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DBI_PEA_Scoring.Common;
using DBI_PEA_Scoring.Model;
using DBI_PEA_Scoring.Model.Teacher;
using DBI_PEA_Scoring.Utils.Dao;

namespace DBI_PEA_Scoring.Utils
{
    public class PaperUtils
    {
        /// <summary>
        ///     Test Schema of 2 DBs.
        /// </summary>
        /// <param name="candidate">Question and requirement to check</param>
        /// <param name="studentId"></param>
        /// <param name="answer">Answer of student</param>
        /// <returns>Result when compare 2 DB</returns>
        /// <exception cref="SqlException">
        ///     When something's wrong, throw exception to log error to KhaoThi
        /// </exception>
        internal static Dictionary<string, string> SchemaType(Candidate candidate, string studentId, string answer, int questionOrder)
        {
            string dbSolutionName = studentId.Replace(" ", "") + questionOrder + "_Solution" + "_" + new Random().Next(1000000000);
            string dbAnswerName = studentId.Replace(" ", "") + "_" + questionOrder + "_Answer" + "_" + new Random().Next(1000000000);
            string dbEmptyName = studentId.Replace(" ", "") + questionOrder + "_EmptyDb" + "_" + new Random().Next(1000000000);
            string querySolution = string.Concat("create database [", dbSolutionName, "]\nGO\nUSE [", dbSolutionName, "]\n", candidate.Solution);
            string queryAnswer = string.Concat("create database [", dbAnswerName, "]\nGO\nUSE [", dbAnswerName, "]\n", answer);
            string queryEmptyDb = string.Concat("create database [", dbEmptyName, "]");

            try
            {
                string errorMessage = "";
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
                    {
                        errorMessage = string.Concat("Answer query error: ", e.Message, "\n");
                    }
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
                return General.CompareTwoDatabases(dbAnswerName, dbSolutionName, dbEmptyName, candidate, errorMessage);
            }
            finally
            {
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
        internal static Dictionary<string, string> SelectType(Candidate candidate, string studentId, string answer, int questionOrder)
        {
            string dbSolutionName = studentId.Replace(" ", "") + questionOrder + "_Solution" + "_" + new Random().Next(1000000000);
            string dbAnswerName = studentId.Replace(" ", "") + questionOrder + "_Answer" + "_" + new Random().Next(1000000000);

            try
            {
                //Generate 2 new DB for student's answer and solution
                if (Constant.PaperSet.DBScriptList.Count > 1)
                {
                    General.GenerateDatabase(dbSolutionName, dbAnswerName, Constant.PaperSet.DBScriptList[1]);
                }
                else
                {
                    General.GenerateDatabase(dbSolutionName, dbAnswerName, Constant.PaperSet.DBScriptList[0]);
                }
                //Compare
                return General.CompareOneResultSet(dbAnswerName, dbSolutionName, answer, candidate);
            }
            finally
            {
                General.DropDatabase(dbSolutionName);
                General.DropDatabase(dbAnswerName);
            }
        }

        /// <summary>
        ///  Execute Query and compare 2 effected tables
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="studentId"></param>
        /// <param name="answer">Answer of student</param>
        /// <param name="questionOrder"></param>
        /// <returns>Result when compare 2 result tables</returns>
        /// <exception cref="Exception">
        ///     When something's wrong, throw exception to log to Khao Thi.
        /// </exception>
        internal static Dictionary<string, string> DMLType(Candidate candidate, string studentId, string answer, int questionOrder)
        {
            string dbSolutionName = studentId.Replace(" ", "") + questionOrder + "_Solution" + "_" + new Random().Next(10000000);
            string dbAnswerName = studentId.Replace(" ", "") + questionOrder + "_Answer" + "_" + new Random().Next(10000000);


            //Generate 2 new DB for student's answer and solution

            //Create db from solution at schema question
            string queryDbForDml;
            if (Constant.PaperSet.DBScriptList.Count > 1)
            {
                queryDbForDml = Constant.PaperSet.DBScriptList[1];
            }
            else
            {
                queryDbForDml = Constant.PaperSet.DBScriptList[0];
            }
            if (candidate.RelatedSchema)
            {
                queryDbForDml = "";
                foreach (var question in Constant.PaperSet.QuestionSet.QuestionList)
                {
                    if (question.Candidates.ElementAt(0).QuestionType == Candidate.QuestionTypes.Schema)
                    {
                        foreach (var candi in question.Candidates)
                        {
                            queryDbForDml = string.Concat(queryDbForDml, "\n", candi.Solution);
                        }
                        break;
                    }

                }
            }

            try
            {
                //Generate databases for solution and answer
                General.GenerateDatabase(dbSolutionName, dbAnswerName, queryDbForDml);

                string errorMessage = "";
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
                return General.CompareMoreResultSets(dbAnswerName, dbSolutionName, candidate, errorMessage);
            }
            finally
            {
                General.DropDatabase(dbSolutionName);
                General.DropDatabase(dbAnswerName);
            }
        }

        /// <summary>
        ///  Execute Query and compare 2 effected tables
        /// </summary>
        /// <param name="dbScript"></param>
        /// <param name="candidate">Question and requirement to check</param>
        /// <param name="studentId"></param>
        /// <param name="answer">Answer of student</param>
        /// <param name="questionOrder"></param>
        /// <returns>Result when compare 2 result tables</returns>
        /// <exception cref="Exception">
        ///     When something's wrong, throw exception to log to Khao Thi.
        /// </exception>
        internal static Dictionary<string, string> TriggerProcedureType(Candidate candidate, string studentId, string answer, int questionOrder)
        {
            string dbSolutionName = studentId.Replace(" ", "") + questionOrder + "_Solution" + "_" + new Random().Next(10000000);
            string dbAnswerName = studentId.Replace(" ", "") + questionOrder + "_Answer" + "_" + new Random().Next(10000000);

            //Generate 2 new DB for student's answer and solution
            if (Constant.PaperSet.DBScriptList.Count > 1)
            {
                General.GenerateDatabase(dbSolutionName, dbAnswerName, Constant.PaperSet.DBScriptList[1]);
            }
            else
            {
                General.GenerateDatabase(dbSolutionName, dbAnswerName, Constant.PaperSet.DBScriptList[0]);
            }
            try
            {
                string errorMessage = "";
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
                return General.CompareMoreResultSets(dbAnswerName, dbSolutionName, candidate, errorMessage);
            }
            finally
            {
                General.DropDatabase(dbSolutionName);
                General.DropDatabase(dbAnswerName);
            }
        }
    }
}