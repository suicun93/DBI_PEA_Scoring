using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using DBI_PEA_Scoring.Model;
using DBI_PEA_Scoring.Utils.DaoType;
using DBI_PEA_Scoring.Common;

namespace DBI_PEA_Scoring.Utils
{
    public class TestUtils
    {
        /// <summary>
        ///     Test Simple Query
        /// </summary>
        /// <param name="candidate">Question and requirement to check</param>
        /// <param name="answer">Answer of student</param>
        /// <returns>Result when compare 2 table</returns>
        /// <exception cref="SqlException">
        /// When something's wrong, throw exception to log error to KhaoThi
        /// </exception>
        internal static bool TestSelect(Candidate candidate, string answer)
        {
            //Duplicate 2 new DB for student and teacher
            General.DuplicatedDb( Constant.listDB[0].SqlServerDbFolder,
                Constant.listDB[0].SourceDbName, "DbForTest");
            string dbTeacherName = "DbForTest_Teacher";
            string dbStudentName = "DbForTest_Student";
            try
            {
                //Marking
                foreach (Requirement req in candidate.Requirements)
                {
                    // In case question type is Query, requirement type default is result set. 
                    if (SelectType.MarkSelectType(req.RequireSort, dbTeacherName, dbStudentName,
                            req.ResultQuery, answer) == false)
                    {
                        General.DropDatabase(dbTeacherName);
                        General.DropDatabase(dbStudentName);
                        return false;
                    }
                }
                General.DropDatabase(dbTeacherName);
                General.DropDatabase(dbStudentName);
                return true;
            }
            catch (SqlException e)
            {
                General.DropDatabase(dbTeacherName);
                General.DropDatabase(dbStudentName);
                throw e;
            }
        }

        /// <summary>
        ///     Test Schema of 2 DBs.
        /// </summary>
        /// <param name="candidate">Question and requirement to check</param>
        /// <param name="answer">Answer of student</param>
        /// <returns>Result when compare 2 DB</returns>
        /// <exception cref="SqlException">
        ///     When something's wrong, throw exception to log error to KhaoThi
        /// </exception>
        internal static bool TestSchema(Candidate candidate, string answer)
        {
            string dbTeacherName = "DbForTest_Teacher";
            string dbStudentName = "DbForTest_Student";
            try
            {
                //Marking
                foreach (Requirement req in candidate.Requirements)
                {
                    if (SchemaType.MarkSchemaDatabasesType(dbTeacherName, dbStudentName,
                        answer, req.ResultQuery) == false)
                    {
                        return false;
                    }
                }
                General.DropDatabase(dbTeacherName);
                General.DropDatabase(dbStudentName);
                return true;
            }
            catch (SqlException e)
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
        /// <exception cref="SqlException">
        ///     When something's wrong, throw exception to log to Khao Thi.
        /// </exception>
        internal static bool TestDML(Candidate candidate, string answer)
        {
            string dbTeacherName = "DbForTest_Teacher";
            string dbStudentName = "DbForTest_Student";
            bool noRequireSort = false;
            try
            {
                // In case question type is InsertDeleteUpdate, requirement type default is Effect. 
                //Marking
                foreach (Requirement req in candidate.Requirements)
                {
                    // Execute query
                    General.ExecuteQuery(dbTeacherName, dbStudentName, req.ActivateTriggerQuery, answer);
                    if (SelectType.MarkSelectType(noRequireSort, dbTeacherName, dbStudentName,
                             req.ResultQuery, req.ResultQuery) == false)
                    {
                        General.DropDatabase(dbTeacherName);
                        General.DropDatabase(dbStudentName);
                        return false;
                    }
                }
                General.DropDatabase(dbTeacherName);
                General.DropDatabase(dbStudentName);
                return true;
            }
            catch (SqlException e)
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
        /// <exception cref="SqlException">
        ///     When something's wrong, throw exception to log to Khao Thi.
        /// </exception>
        internal static bool TestProcedure(Candidate candidate, string answer)
        {
            try
            {
                return true;
            }
            catch (SqlException e)
            {

                throw e;
            }

        }
        internal static bool TestTrigger(Candidate candidate, string answer)
        {
            throw new NotImplementedException();
        }
    }
}
