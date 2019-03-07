using DBI_PEA_Scoring.Model;
using System;

namespace DBI_PEA_Scoring.Utils.DaoType
{
    class TriggerType
    {
        /// <summary>
        ///     Get mark for trigger question
        /// </summary>
        /// <param name="dbStudentName">Name of student DB</param>
        /// <param name="dbTeacherName">Name of teacher DB</param>
        /// <param name="queryStudent">Query of student</param>
        /// <param name="candidate">Query of teacher and query to check</param>
        /// <returns>True if ok and throw exception if something wrong</returns>
        public static bool MarkTriggerTest(string dbStudentName, string dbTeacherName,
            string queryStudent, Candidate candidate)
        {
            // Run solution of both
            string queryStudentRun = "Use " + dbStudentName + "\nGO \n" + queryStudent;
            string queryTeacherRun = "Use " + dbTeacherName + "\nGO \n" + candidate.Solution;
            try
            {
                General.ExecuteSingleQuery(queryStudentRun);
            }
            catch (Exception e)
            {
                throw new Exception("Student wrong: " + e.Message);
            }
            try
            {
                General.ExecuteSingleQuery(queryTeacherRun);
            }
            catch (Exception e)
            {
                throw new Exception("Teacher wrong: " + e.Message);
            }
            // Compare
            return General.CompareMoreThanOneTableSort(dbStudentName, dbTeacherName, candidate.TestQuery);
        }
    }
}