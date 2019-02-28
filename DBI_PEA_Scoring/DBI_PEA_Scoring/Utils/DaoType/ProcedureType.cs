using DBI_PEA_Scoring.Model;

namespace DBI_PEA_Scoring.Utils.DaoType
{
    class ProcedureType
    {
        /// <summary>
        ///     Get mark for procedure question
        /// </summary>
        /// <param name="dbStudentName">Name of student DB</param>
        /// <param name="dbTeacherName">Name of teacher DB</param>
        /// <param name="queryStudent">Query of student</param>
        /// <param name="candidate">Query of teacher and query to check</param>
        /// <returns>True if ok and throw exception if something wrong</returns>
        public static bool MarkProcedureTest(string dbTeacherName, string dbStudentName, string queryStudent,
            Candidate candidate)
        {
            // Run solution of both
            var builder = Common.Constant.SqlConnectionStringBuilder;
            string queryStudentRun = "Use " + dbStudentName + "\nGO \n" + queryStudent;
            string queryTeacherRun = "Use " + dbTeacherName + "\nGO \n" + candidate.Solution;
            General.ExecuteSingleQuery(queryStudentRun);
            General.ExecuteSingleQuery(queryTeacherRun);
            // Compare
            return General.CompareMoreThanOneTableSort(dbStudentName, dbTeacherName, candidate.TestQuery);
        }
    }
}
