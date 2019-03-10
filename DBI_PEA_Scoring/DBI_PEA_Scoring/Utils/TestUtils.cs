using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DBI_PEA_Scoring.Model;
using DBI_PEA_Scoring.Utils.DaoType;

namespace DBI_PEA_Scoring.Utils
{
    public class TestUtils
    {
        /// <summary>
        ///     Test Schema of 2 DBs.
        /// </summary>
        /// <param name="candidate">Question and requirement to check</param>
        /// <param name="answer">Answer of student</param>
        /// <returns>Result when compare 2 DB</returns>
        /// <exception cref="SqlException">
        ///     When something's wrong, throw exception to log error to KhaoThi
        /// </exception>
        internal static Dictionary<string, string> TestSchema(Candidate candidate, string studentId, string answer, int questionNumber)
        {
            string dbSolutionName = studentId + "_" + questionNumber + "_" + "Solution";
            string dbAnswerName = studentId + "_" + questionNumber + "_" + "Answer";
            string querySolution = candidate.Solution.Replace(candidate.DBName, dbSolutionName);
            string queryAnswer = answer.Replace(candidate.DBName, dbAnswerName);
            try
            {
                return SchemaType.MarkSchemaDatabasesType(dbAnswerName, dbSolutionName, queryAnswer, querySolution, candidate);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                General.DropDatabase(dbSolutionName);
                General.DropDatabase(dbAnswerName);
            }
        }

        /// <summary>
        ///     Test Simple Query
        /// </summary>
        /// <param name="candidate">Question and requirement to check</param>
        /// <param name="answer">Answer of student</param>
        /// <returns>Result when compare 2 table</returns>
        /// <exception cref="Exception">
        ///     When something's wrong, throw exception to log error to KhaoThi
        /// </exception>
        internal static Dictionary<string, string> TestSelect(Candidate candidate, string studentId, string answer, int questionOrder)
        {
            string dbSolutionName = studentId + "_" + questionOrder + "_Solution";
            string dbAnswerName = studentId + "_" + questionOrder + "_Answer";
            //Generate 2 new DB for student's answer and solution
            General.GenerateDatabase(dbSolutionName, dbAnswerName);
            try
            {
                return SelectType.MarkSelectType(candidate, answer, dbSolutionName, dbAnswerName);
            }
            catch (Exception e)
            {
                throw e;
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
        /// <param name="candidate">Question and requirement to check</param>
        /// <param name="answer">Answer of student</param>
        /// <returns>Result when compare 2 result tables</returns>
        /// <exception cref="Exception">
        ///     When something's wrong, throw exception to log to Khao Thi.
        /// </exception>
        internal static Dictionary<string, string> TestInsertDeleteUpdate(Candidate candidate, string studentId, string answer, int questionOrder)
        {
            string dbSolutionName = studentId + "_" + questionOrder + "_Solution";
            string dbAnswerName = studentId + "_" + questionOrder + "_Answer";

            //Generate 2 new DB for student's answer and solution
            General.GenerateDatabase(dbSolutionName, dbAnswerName);
            try
            {
                // In case question type is DML, Just 1 requirement type, default is result set. 
                // Run query and compare table.
                return DmlType.MarkDMLQuery(dbAnswerName, dbSolutionName, answer, candidate);
            }
            catch (Exception e)
            {
                throw e;
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
        /// <param name="candidate">Question and requirement to check</param>
        /// <param name="answer">Answer of student</param>
        /// <returns>Result when compare 2 result tables</returns>
        /// <exception cref="Exception">
        ///     When something's wrong, throw exception to log to Khao Thi.
        /// </exception>
        internal static Dictionary<string, string> TestProcedure(Candidate candidate, string studentId, string answer, int questionOrder)
        {
            string dbSolutionName = studentId + "_" + questionOrder + "_Solution";
            string dbAnswerName = studentId + "_" + questionOrder + "_Answer";

            //Generate 2 new DB for student's answer and solution
            General.GenerateDatabase(dbSolutionName, dbAnswerName);
            try
            {
                // In case question type is DML, Just 1 requirement type, default is result set. 
                // Run query and compare table.
                return ProcedureType.MarkProcedureTest(dbAnswerName, dbSolutionName, answer, candidate);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                General.DropDatabase(dbSolutionName);
                General.DropDatabase(dbAnswerName);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="candidate">Solution of teacher</param>
        /// <param name="answer">Answer of student</param>
        /// <returns>Result when compare 2 result tables</returns>
        /// /// <exception cref="Exception">
        ///     When something's wrong, throw exception to log to Khao Thi.
        /// </exception>
        internal static Dictionary<string, string> TestTrigger(Candidate candidate, string studentId, string answer, int questionOrder)
        {
            string dbSolutionName = studentId + "_" + questionOrder + "_Solution";
            string dbAnswerName = studentId + "_" + questionOrder + "_Answer";

            //Generate 2 new DB for student's answer and solution
            General.GenerateDatabase(dbSolutionName, dbAnswerName);
            try
            {
                // In case question type is DML, Just 1 requirement type, default is result set. 
                // Run query and compare table.
                return TriggerType.MarkTriggerTest(dbAnswerName, dbSolutionName, answer, candidate);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                General.DropDatabase(dbSolutionName);
                General.DropDatabase(dbAnswerName);
            }
        }
    }
}