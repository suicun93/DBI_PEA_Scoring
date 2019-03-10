using DBI_PEA_Scoring.Model;
using System;
using System.Collections.Generic;

namespace DBI_PEA_Scoring.Utils.DaoType
{
    class ProcedureType
    {
        /// <summary>
        ///     Get mark for procedure question
        /// </summary>
        /// <param name="dbAnswerName">Name of student DB</param>
        /// <param name="dbSolutionName">Name of teacher DB</param>
        /// <param name="answer">Query of student</param>
        /// <param name="candidate">Query of teacher and query to check</param>
        /// <returns>True if ok and throw exception if something wrong</returns>
        public static Dictionary<string, string> MarkProcedureTest(string dbAnswerName, string dbSolutionName, string answer, Candidate candidate)
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
            }
            catch (Exception e)
            {
                throw new Exception("Solution error: " + e.Message + "Query: " + candidate.Solution);
            }
            // Compare nosort and return result(T/F)
            if (General.CompareMoreThanOneTableSort(dbAnswerName, dbSolutionName, candidate.TestQuery))
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
