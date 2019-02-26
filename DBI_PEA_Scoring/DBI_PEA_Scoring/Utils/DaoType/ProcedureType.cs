﻿using DBI_PEA_Scoring.Model;

namespace DBI_PEA_Scoring.Utils.DaoType
{
    class ProcedureType
    {
        public static bool MarkProcedureTest(string dbTeacherName, string dbStudentName, Candidate candi,
            string queryStudent)
        {
            //Run solution of both
            var builder = Common.Constant.SqlConnectionStringBuilder;
            General.ExecuteQuery(dbStudentName, dbTeacherName, queryStudent, candi.Solution);
            string activeQuery = candi.ActivateQuery;
            //Check Requirement Type
            foreach(Requirement require in candi.Requirements)
            {
                switch (require.Type)
                {
                    case Requirement.RequirementTypes.Effect:
                        //Run active query
                        General.ExecuteQuery(dbStudentName, dbTeacherName, activeQuery, activeQuery);
                        //Run check effect query to compare result
                        return General.CompareTableNoSort(dbTeacherName, dbStudentName, 
                            require.CheckEffectQuery, require.CheckEffectQuery);
                    case Requirement.RequirementTypes.Parameter:
                        //Run active query
                        General.ExecuteQuery(dbStudentName, dbTeacherName, activeQuery, activeQuery);
                        //Run check effect query to compare result
                        return General.CompareTableNoSort(dbTeacherName, dbStudentName, 
                            require.CheckEffectQuery, require.CheckEffectQuery);
                    case Requirement.RequirementTypes.ResultSet:
                        return General.CompareTableNoSort(dbTeacherName, dbStudentName, activeQuery,
                            activeQuery);
                }
            }
            return false;
        }
    }
}
