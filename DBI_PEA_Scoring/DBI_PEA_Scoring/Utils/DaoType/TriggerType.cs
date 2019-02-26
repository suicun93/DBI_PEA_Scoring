using DBI_PEA_Scoring.Common;
using DBI_PEA_Scoring.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBI_PEA_Scoring.Utils.DaoType
{
    class TriggerType
    {
        public static bool MarkTriggerTest(string dbTeacherName, string dbStudentName, 
            Candidate candi, string queryStudent)
        {
            //Run solution of both
            var builder = Constant.SqlConnectionStringBuilder;
            General.ExecuteQuery(dbStudentName, dbTeacherName, queryStudent, candi.Solution);
            string activeQuery = candi.ActivateQuery;
            //Check Requirement Type
            foreach (Requirement require in candi.Requirements)
            {
                switch (require.Type)
                {
                    case Requirement.RequirementTypes.Effect:
                        //Run active query
                        General.ExecuteQuery(dbStudentName, dbTeacherName, activeQuery, activeQuery);
                        //Run check effect query to compare result
                        return General.CompareTableNoSort(dbTeacherName, dbStudentName,
                            require.CheckEffectQuery, require.CheckEffectQuery);
                    case Requirement.RequirementTypes.ResultSet:
                        //Run active query then compare result
                        return General.CompareTableNoSort(dbTeacherName, dbStudentName, activeQuery,
                            activeQuery);
                }
            }
            return false;
        }
    }
}
