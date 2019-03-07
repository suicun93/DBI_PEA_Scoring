using DBI_PEA_Scoring.Common;
using System;
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
        public static bool MarkDMLQuery(string dbStudentName, string dbTeacherName,
            string queryEffectStudent, string queryEffectTeacher, string queryToCheck)
        {
            // After run query, compare table (no sort)
            // Execute query
            string queryStudent = "USE " + dbStudentName + "\nGO \n" + queryEffectStudent;
            string queryTeacher = "USE " + dbTeacherName + "\nGO \n" + queryEffectTeacher;
            try
            {
                General.ExecuteSingleQuery(queryStudent);
            }
            catch (Exception e)
            {
                throw new Exception("Student wrong: " + e.Message);
            }
            try
            {
                General.ExecuteSingleQuery(queryTeacher);
            }
            catch (Exception e)
            {
                throw new Exception("Teacher wrong: " + e.Message);
            }
            // Compare nosort and return result(T/F)
            return General.CompareMoreThanOneTableSort(dbStudentName, dbTeacherName,
                queryToCheck);
        }
    }
}
