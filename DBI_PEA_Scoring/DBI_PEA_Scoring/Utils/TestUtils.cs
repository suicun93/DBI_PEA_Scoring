using System;
using System.Data.SqlClient;
using DBI_PEA_Scoring.Model;
using DBI_PEA_Scoring.Utils.DaoType;

namespace DBI_PEA_Scoring.Utils
{
    public class TestUtils
    {
        public static Common.Database Database = null;

        /// <summary>
        ///     Test Schema of 2 DBs.
        /// </summary>
        /// <param name="candidate">Question and requirement to check</param>
        /// <param name="answer">Answer of student</param>
        /// <returns>Result when compare 2 DB</returns>
        /// <exception cref="SqlException">
        ///     When something's wrong, throw exception to log error to KhaoThi
        /// </exception>
        internal static bool TestSchema(Candidate candidate, string studentId, string answer, int questionOrder)
        {
            //string dbName = studentId + "_" + questionOrder + "_";
            //string dbTeacherName = dbName + "dbTeacherName";
            //string dbStudentName = dbName + "dbStudentName";
            //string queryTeacher = candidate.Solution.Replace(candidate.DBName, dbTeacherName);
            //string queryStudent = answer.Replace(candidate.DBName, dbStudentName);
            //try
            //{
            //    // Only check by compare 2 DB
            //    if (SchemaType.MarkSchemaDatabasesType(dbStudentName, dbTeacherName,
            //        queryStudent, queryTeacher) == false)
            //    {
            //        General.DropDatabase(dbTeacherName);
            //        General.DropDatabase(dbStudentName);
            //        return false;
            //    }
            //    General.DropDatabase(dbTeacherName);
            //    General.DropDatabase(dbStudentName);
            //    return true;
            //}
            //catch (SqlException e)
            //{
            //    General.DropDatabase(dbTeacherName);
            //    General.DropDatabase(dbStudentName);
            //    throw e;
            //}
            return true;
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
        internal static bool TestSelect(Candidate candidate, string studentId, string answer, int questionOrder)
        {
            string dbName = studentId + "_" + questionOrder;
            string dbTeacherName = dbName + "_Teacher";
            string dbStudentName = dbName + "_Student";
            //Duplicate 2 new DB for student and teacher
            General.DuplicatedDb(Database.SqlServerDbFolder,
                Database.SourceDbName, dbName);
            try
            {
                // In case question type is Query, Just 1 requirement type, default is result set. 
                // We will run solution to check
                if (SelectType.MarkSelectType(candidate.RequireSort, dbStudentName, dbTeacherName, answer, candidate.Solution) == false)
                {
                    throw new Exception("Student's result and teacher's result are not the same.");
                }
                General.DropDatabase(dbTeacherName);
                General.DropDatabase(dbStudentName);
                return true;
            }
            catch (Exception e)
            {
                General.DropDatabase(dbTeacherName);
                General.DropDatabase(dbStudentName);
                throw e;
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
        internal static bool TestInsertDeleteUpdate(Candidate candidate, string studentId, string answer, int questionOrder)
        {
            string dbName = studentId + "_" + questionOrder;
            string dbTeacherName = dbName + "_Teacher";
            string dbStudentName = dbName + "_Student";

            //Duplicate 2 new DB for student and teacher
            General.DuplicatedDb(Database.SqlServerDbFolder,
                Database.SourceDbName, dbName);
            try
            {
                // In case question type is DML, Just 1 requirement type, default is result set. 
                // Run query and compare table.
                string checkQuery = candidate.TestQuery;
                if (DmlType.MarkDMLQuery(dbStudentName, dbTeacherName, answer, candidate.Solution, checkQuery) == false)
                {
                    throw new System.Exception("Student's result and teacher's result are not the same.");
                }
                General.DropDatabase(dbTeacherName);
                General.DropDatabase(dbStudentName);
                return true;
            }
            catch (Exception e)
            {
                General.DropDatabase(dbTeacherName);
                General.DropDatabase(dbStudentName);
                throw e;
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
        internal static bool TestProcedure(Candidate candidate, string studentId, string answer, int questionOrder)
        {
            string dbName = studentId + "_" + questionOrder;
            string dbTeacherName = dbName + "_Teacher";
            string dbStudentName = dbName + "_Student";
            //Duplicate 2 new DB for student and teacher
            General.DuplicatedDb(Database.SqlServerDbFolder,
                Database.SourceDbName, dbName);
            try
            {
                // Create procedure then compare 2 multiple result sets by TestQuery
                if (ProcedureType.MarkProcedureTest(dbStudentName, dbTeacherName, answer,
                    candidate) == false)
                {
                    throw new System.Exception("Student and teacher's result are not matched.");
                }
                General.DropDatabase(dbTeacherName);
                General.DropDatabase(dbStudentName);
                return true;
            }
            catch (Exception e)
            {
                General.DropDatabase(dbTeacherName);
                General.DropDatabase(dbStudentName);
                throw e;
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
        internal static bool TestTrigger(Candidate candidate, string studentId, string answer, int questionOrder)
        {
            string dbName = studentId + "_" + questionOrder;
            string dbTeacherName = dbName + "_Teacher";
            string dbStudentName = dbName + "_Student";
            //Duplicate 2 new DB for student and teacher
            General.DuplicatedDb(Database.SqlServerDbFolder,
                Database.SourceDbName, dbName);
            try
            {
                // Create trigger then compare 2 multiple result sets by TestQuery
                if (TriggerType.MarkTriggerTest(dbStudentName, dbTeacherName, answer,
                    candidate) == false)
                {
                    throw new System.Exception("Student and teacher's result are not matched.");
                }
                General.DropDatabase(dbTeacherName);
                General.DropDatabase(dbStudentName);
                return true;
            }
            catch (Exception e)
            {
                General.DropDatabase(dbTeacherName);
                General.DropDatabase(dbStudentName);
                throw e;
            }
        }
    }
}