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
            string dbSolutionName = studentId + "_" + questionOrder + "_Solution" + "_" + new Random().Next(1000000000);
            string dbAnswerName = studentId + "_" + questionOrder + "_Answer" + "_" + new Random().Next(1000000000);
            string querySolution = string.Concat("create database ", dbSolutionName, "\nGO\nUSE ", dbSolutionName, "\n", answer);
            string queryAnswer = string.Concat("create database ", dbAnswerName, "\nGO\nUSE ", dbAnswerName, "\n", candidate.Solution);
            try
            {
                // Execute query
                try
                {
                    General.ExecuteSingleQuery(queryAnswer, "master");
                }
                catch (Exception e)
                {
                    throw new Exception("Answer error: " + e.Message);
                }
                try
                {
                    General.ExecuteSingleQuery(querySolution, "master");
                }
                catch (Exception e)
                {
                    throw new Exception("Solution error: " + e.Message);
                }
                // Execute query
                return General.CompareTwoDatabases(dbAnswerName, dbSolutionName, candidate);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                General.DropDatabase(dbAnswerName);
                General.DropDatabase(dbSolutionName);
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
            string dbSolutionName = studentId + "_" + questionOrder + "_Solution" + "_" + new Random().Next(1000000000);
            string dbAnswerName = studentId + "_" + questionOrder + "_Answer" + "_" + new Random().Next(1000000000);
            //Generate 2 new DB for student's answer and solution
            General.GenerateDatabase(dbSolutionName, dbAnswerName, Constant.PaperSet.DBScriptList[1]);
            try
            {
                return General.CompareOneResultSet(dbAnswerName, dbSolutionName, answer, candidate);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
        /// <param name="studentId"></param>
        /// <param name="answer">Answer of student</param>
        /// <param name="questionOrder"></param>
        /// <returns>Result when compare 2 result tables</returns>
        /// <exception cref="Exception">
        ///     When something's wrong, throw exception to log to Khao Thi.
        /// </exception>
        internal static Dictionary<string, string> DMLType(Candidate candidate, string studentId, string answer, int questionOrder)
        {
            string dbSolutionName = studentId + "_" + questionOrder + "_Solution" + "_" + new Random().Next(10000000);
            string dbAnswerName = studentId + "_" + questionOrder + "_Answer" + "_" + new Random().Next(10000000);


            //Generate 2 new DB for student's answer and solution

            //Create db from solution at schema question
            string queryDbForDml = Constant.PaperSet.DBScriptList[1];

            if (candidate.RelatedSchema)
            {
                queryDbForDml = "";
                foreach (var question in Constant.PaperSet.QuestionSet.QuestionList)
                {
                    foreach (var candi in question.Candidates)
                    {
                        if (candi.QuestionType == Candidate.QuestionTypes.Schema)
                            queryDbForDml = string.Concat(queryDbForDml, candi.Solution, "\n");
                    }
                }
            }

            //Generate database for check
            General.GenerateDatabase(dbSolutionName, dbAnswerName, queryDbForDml);

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
                    errorMessage = string.Concat(errorMessage, "Answer error: " + e.Message + "\n");
                    //Still grading for student even error
                    //Student still right at some testcase, need to keep grading
                }
                try
                {
                    General.ExecuteSingleQuery(candidate.Solution, dbSolutionName);
                }
                catch (Exception e)
                {
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
            string dbSolutionName = studentId + "_" + questionOrder + "_Solution" + "_" + new Random().Next(10000000);
            string dbAnswerName = studentId + "_" + questionOrder + "_Answer" + "_" + new Random().Next(10000000);

            //Generate 2 new DB for student's answer and solution
            General.GenerateDatabase(dbSolutionName, dbAnswerName, Constant.PaperSet.DBScriptList[1]);
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
                    errorMessage = string.Concat(errorMessage, "Answer error: " + e.Message + "\n");
                    //Still grading for student even error
                    //Student still right at some testcase, need to keep grading
                }
                try
                {
                    General.ExecuteSingleQuery(candidate.Solution, dbSolutionName);
                }
                catch (Exception e)
                {
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