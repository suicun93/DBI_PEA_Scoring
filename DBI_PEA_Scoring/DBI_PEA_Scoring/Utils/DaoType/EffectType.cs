using System.Data.SqlClient;

namespace DBI_PEA_Scoring.Utils.DaoType
{
    class EffectType
    {
        readonly SqlConnectionStringBuilder _builder;

        /// <summary>
        /// Init connection
        /// </summary>
        /// <param name="dataSource">(something like localhost)</param> 
        /// <param name="userId">(sa)</param> 
        /// <param name="password">(123)</param> 
        /// <param name="initialCatalog">(master)</param> 
        public EffectType(string dataSource, string userId, string password, string initialCatalog)
        {
            // Build connection string
            _builder = new SqlConnectionStringBuilder
            {
                DataSource = dataSource,
                UserID = userId,
                Password = password,
                InitialCatalog = initialCatalog
            };
        }

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
        public bool MarkInsUpDelTypeQuery(string dbTeacherName, string dbStudentName,
            string queryEffectTeacher, string queryEffectStudent, string queryCheckEffect)
        {
            queryEffectTeacher = "use " + dbTeacherName + "\n" + queryEffectTeacher + "";
            queryEffectStudent = "use " + dbStudentName + "\n" + queryEffectStudent + "";
            SelectType selectType = new SelectType(_builder);
            using (SqlConnection connection = new SqlConnection(_builder.ConnectionString))
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
                return selectType.CompareTableNoSort(dbStudentName, dbTeacherName,
                    queryCheckEffect, queryCheckEffect, _builder);
            }
        }
    }
}
