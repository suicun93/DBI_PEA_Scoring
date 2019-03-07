
namespace DBI_PEA_Scoring.Utils.DaoType
{
    public class SelectType
    {
        /// <summary>
        /// Marking Query Select type
        /// </summary>
        /// <param name="isSort">is this question need sorting</param>
        /// <param name="dbTeacherName"></param>
        /// <param name="dbStudentName"></param>
        /// <param name="queryTeacher"></param>
        /// <param name="queryStudent"></param>
        /// <returns></returns>
        public static bool MarkSelectType(bool isSort, string dbStudentName, string dbTeacherName, string queryStudent,
            string queryTeacher)
        {
            switch (isSort)
            {
                case true:
                    //return General.CompareOneTableNoSort(dbTeacherName, dbStudentName, queryTeacher, queryStudent);
                    return General.CompareMoreThanOneTableSort(dbStudentName, dbTeacherName, queryStudent, queryTeacher);
                case false:
                    return General.CompareMoreThanOneTableSort(dbStudentName, dbTeacherName, queryStudent, queryTeacher);
                default:
                    return false;
            }
        }
    }
}
