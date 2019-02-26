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
        public bool MarkDMLQuery(string dbTeacherName, string dbStudentName,
            string queryEffectTeacher, string queryEffectStudent, string queryCheckEffect)
        {
            var builder = Constant.SqlConnectionStringBuilder;
            queryEffectTeacher = "use " + dbTeacherName + "\n" + queryEffectTeacher + "";
            queryEffectStudent = "use " + dbStudentName + "\n" + queryEffectStudent + "";
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                //Run Effect query
                using (SqlCommand cmdEffStudent = new SqlCommand(queryEffectStudent, connection))
                {
                    cmdEffStudent.ExecuteNonQuery();
                }
                using (SqlCommand cmdEffTeacher = new SqlCommand(queryEffectTeacher, connection))
                {
                    cmdEffTeacher.ExecuteNonQuery();
                }
                return General.CompareTableNoSort(dbStudentName, dbTeacherName,
                    queryCheckEffect, queryCheckEffect);
            }
        }
    }
}
