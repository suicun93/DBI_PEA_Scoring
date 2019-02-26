using DBI_PEA_Scoring.Common;
using DBI_PEA_Scoring.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBI_PEA_Scoring.Utils.DaoType
{
    class ProcedureType
    {
        public bool MarkProcedureTest(string dbTeacherName, string dbStudentName, string queryTeacher,
            string queryStudent)
        {
            //Run solution of both
            var builder = Constant.SqlConnectionStringBuilder;
            General.ExecuteQuery(dbStudentName, dbTeacherName, queryStudent, queryTeacher);
            //Check 
            return false;
        }
    }
}
