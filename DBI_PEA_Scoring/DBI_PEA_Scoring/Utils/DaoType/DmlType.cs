using DBI_PEA_Scoring.Common;
using DBI_PEA_Scoring.Model;
using System;
using System.Collections.Generic;
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
        /// <param name="dbSolutionName">database for teacher query</param>
        /// <param name="dbAnswerName">database for student query</param>
        /// <param name="queryEffectTeacher">query effect on table from teacher</param>
        /// <param name="answer">query effect on table from student</param>
        /// <param name="queryCheckEffect">query answer to check table effected from teacher</param>
        /// <returns></returns>
        public static Dictionary<string, string> MarkDMLQuery(string dbAnswerName, string dbSolutionName, string answer, Candidate candidate)
        {
            // Execute query
            try
            {
                General.ExecuteQuery(answer, dbAnswerName);
            }
            catch (Exception e)
            {
                throw new Exception("Answer error: " + e.Message + "Querry: " + answer);
            }
            try
            {
                General.ExecuteQuery(candidate.Solution, dbSolutionName);
            }
            catch (Exception e)
            {
                throw new Exception("Solution error: " + e.Message + "Query: " + candidate.Solution);
            }
            // Compare nosort and return result(T/F)
            if(General.CompareMoreThanOneTableSort(dbAnswerName, dbSolutionName, candidate.TestQuery))
            {
                return new Dictionary<string, string>()
                                                {
                                                    {"Point", candidate.Point.ToString()},
                                                    {"Comment", "True"},
                                                };
            }
            return null;
        }
    }
}
