using DBI_PEA_Scoring.Common;
using System.Data.SqlClient;

namespace DBI_PEA_Scoring.Utils.DaoType
{
    class DmlType
    {
        ///////////////////////////////////////////////////
        ////Insert, Update, Delete statements//////////////
        ///////////////////////////////////////////////////

        /// <summary>
        /// Insert Update Delete Type
        /// </summary>
        /// <param name="dbTeacherName">database for teacher query</param>
        /// <param name="dbStudentName">database for student query</param>
        /// <param name="queryEffectTeacher">query effect on table from teacher</param>
        /// <param name="queryEffectStudent">query effect on table from student</param>
        /// <param name="queryCheckEffect">query answer to check table effected from teacher</param>
        /// <returns></returns>
        public static bool MarkDMLQuery(string dbTeacherName, string dbStudentName,
            string queryEffectTeacher, string queryEffectStudent, string queryToCheck)
        {
            var builder = Constant.SqlConnectionStringBuilder;
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                // After run query, compare table (no sort)
                // Execute query
                General.ExecuteQuery(dbStudentName, dbTeacherName, queryEffectStudent, queryEffectTeacher);
                // Compare nosort and return result(T/F)
                return General.CompareOneTableNoSort(dbStudentName, dbTeacherName,
                    queryToCheck, queryToCheck);
            }
        }
    }
}
