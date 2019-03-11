using DBI_PEA_Scoring.Model;
using System;
using System.Collections.Generic;

namespace DBI_PEA_Scoring.Utils.DaoType
{
    class TriggerType
    {
        /// <summary>
        ///     Get mark for trigger question
        /// </summary>
        /// <param name="dbStudentName">Name of student DB</param>
        /// <param name="dbTeacherName">Name of teacher DB</param>
        /// <param name="queryStudent">Query of student</param>
        /// <param name="candidate">Query of teacher and query to check</param>
        /// <returns>True if ok and throw exception if something wrong</returns>
        public static Dictionary<string, string> MarkTriggerTest(string dbAnswerName, string dbSolutionName, string answer, Candidate candidate)
        {
            // Execute query
            try
            {
                General.ExecuteQuery(answer, dbAnswerName);
            }
            catch (Exception e)
            {
                throw new Exception("Answer error: " + e.Message + "Query: " + answer);
            }
            try
            {
                General.ExecuteQuery(candidate.Solution, dbSolutionName);
                // Compare nosort and return result(T/F)
                if (General.CompareMoreThanOneTableSort(dbAnswerName, dbSolutionName, candidate.TestQuery))
                {
                    return new Dictionary<string, string>()
                                                {
                                                    {"Point", candidate.Point.ToString()},
                                                    {"Comment", "True"},
                                                };
                }
            }
            catch (Exception e)
            {
                throw new Exception("Solution error: " + e.Message + "Query: " + candidate.Solution);
            }
            return new Dictionary<string, string>()
                                                {
                                                    {"Point", "0"},
                                                    {"Comment", "False"},
                                                }; ;
        }
    }
}