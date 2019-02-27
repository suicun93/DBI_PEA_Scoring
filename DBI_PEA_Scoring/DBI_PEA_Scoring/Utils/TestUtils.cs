using System.Data.SqlClient;
using DBI_PEA_Scoring.Model;
using DBI_PEA_Scoring.Utils.DaoType;

namespace DBI_PEA_Scoring.Utils
{
    public class TestUtils
    {
        static Common.Database database = Common.Constant.listDB[0];

        /// <summary>
        ///     Test Simple Query
        /// </summary>
        /// <param name="candidate">Question and requirement to check</param>
        /// <param name="answer">Answer of student</param>
        /// <returns>Result when compare 2 table</returns>
        /// <exception cref="SqlException">
        ///     When something's wrong, throw exception to log error to KhaoThi
        /// </exception>
        internal static bool TestSelect(Candidate candidate, string answer)
        {
            //Duplicate 2 new DB for student and teacher
            General.DuplicatedDb(database.SqlServerDbFolder,
                database.SourceDbName);
            string dbTeacherName = Common.Constant.newDBName + "_Teacher";
            string dbStudentName = Common.Constant.newDBName + "_Student";
            try
            {
                // In case question type is Query, Just 1 requirement type, default is result set. 
                // We will run solution to check
                if (SelectType.MarkSelectType(candidate.Requirements[0].RequireSort, dbStudentName, dbTeacherName, answer, candidate.Solution) == false)
                {
                    General.DropDatabase(dbTeacherName);
                    General.DropDatabase(dbStudentName);
                    return false;
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
        internal static bool TestSchema(Candidate candidate, string answer, string dbName)
        {
            string dbTeacherName = "DbForTest_Teacher";
            string dbStudentName = "DbForTest_Student";
            // replace by dbName
            // query teacher = candidate.Solution.Replace(dbName,dbTeacherName)
            // query student = answer.Replace(dbName,dbTeacherName)
            try
            {
                // Only check by compare 2 DB
                if (SchemaType.MarkSchemaDatabasesType(dbStudentName, dbTeacherName,
                    answer, candidate.Solution) == false)
                {
                    General.DropDatabase(dbTeacherName);
                    General.DropDatabase(dbStudentName);
                    return false;
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
            //Duplicate 2 new DB for student and teacher
            General.DuplicatedDb(database.SqlServerDbFolder,
                database.SourceDbName);
            string dbTeacherName = Common.Constant.newDBName + "_Teacher";
            string dbStudentName = Common.Constant.newDBName + "_Student";
            try
            {
                // In case question type is DML, Just 1 requirement type, default is result set. 
                // Run query and compare table.
                string checkQuery = candidate.Requirements[0].CheckEffectQuery;
                if (DmlType.MarkDMLQuery(dbStudentName, dbTeacherName, answer, candidate.Solution, checkQuery) == false)
                {
                    General.DropDatabase(dbTeacherName);
                    General.DropDatabase(dbStudentName);
                    return false;
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
            //Duplicate 2 new DB for student and teacher
            General.DuplicatedDb(database.SqlServerDbFolder,
                database.SourceDbName);
            string dbTeacherName = Common.Constant.newDBName + "_Teacher";
            string dbStudentName = Common.Constant.newDBName + "_Student";
            try
            {
                // In case question type is InsertDeleteUpdate, requirement type default is Effect. 
                //Marking
                foreach (Requirement req in candidate.Requirements)
                {
                    // Execute query
                    if (ProcedureType.MarkProcedureTest(dbTeacherName, dbStudentName, candidate,
                        answer) == false)
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
        /// 
        /// </summary>
        /// <param name="candidate">Solution of teacher</param>
        /// <param name="answer">Answer of student</param>
        /// <returns>Result when compare 2 result tables</returns>
        /// /// <exception cref="SqlException">
        ///     When something's wrong, throw exception to log to Khao Thi.
        /// </exception>
        internal static bool TestTrigger(Candidate candidate, string answer)
        {
            //Duplicate 2 new DB for student and teacher
            General.DuplicatedDb(database.SqlServerDbFolder,
                database.SourceDbName);
            string dbTeacherName = Common.Constant.newDBName + "_Teacher";
            string dbStudentName = Common.Constant.newDBName + "_Student";
            try
            {
                // In case question type is InsertDeleteUpdate, requirement type default is Effect. 
                //Marking
                foreach (Requirement req in candidate.Requirements)
                {
                    // Execute query
                    if (TriggerType.MarkTriggerTest(dbTeacherName, dbStudentName, candidate,
                        answer) == false)
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
    }
}