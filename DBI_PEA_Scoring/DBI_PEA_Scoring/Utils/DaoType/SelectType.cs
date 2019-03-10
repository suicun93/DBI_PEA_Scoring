
using DBI_PEA_Scoring.Common;
using DBI_PEA_Scoring.Model;
using System.Collections.Generic;

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
        public static Dictionary<string, string> MarkSelectType(Candidate candidate, string answer, string dbSolutionName, string dbAnswerName)
        {
            string result = "";
            double point = candidate.Point;
            //Compare
            if(General.CompareOneTable(dbAnswerName, dbSolutionName, answer, candidate.Solution))
            {
                result = "True";
                string compareColumnsName = General.CompareOneTableColumnsName(dbAnswerName, dbSolutionName, answer, candidate.Solution);
                if (!string.IsNullOrEmpty(compareColumnsName))
                {
                    result = compareColumnsName;
                    point -= Constant.minusPoint;
                }
            }
            else
            {
                result = string.Concat("Wrong answer:\n","\tAnswer:\n",answer,"\n\tSolution:\n",candidate.Solution,"\n");
                point = 0;
            }
            return new Dictionary<string, string>()
                                                {
                                                    {"Point", point.ToString()},
                                                    {"Comment", result},
                                                };
        }
    }
}
