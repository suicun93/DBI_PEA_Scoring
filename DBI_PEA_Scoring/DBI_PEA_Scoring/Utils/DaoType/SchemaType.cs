using DBI_PEA_Scoring.Common;
using DBI_PEA_Scoring.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DBI_PEA_Scoring.Utils.DaoType
{
    partial class SchemaType
    {
        /// <summary>
        /// Mark Student query
        /// </summary>
        /// <param name="dbSolutionName"></param>
        /// <param name="dbAnswerName"></param>
        /// <param name="solution">query from Teacher</param>
        /// <param name="answer">query from Student</param>
        /// <returns>
        /// "true" if correct
        /// "false" if wrong
        /// message error from sqlserver if error</returns>
        public static Dictionary<string, string> MarkSchemaDatabasesType(string dbAnswerName, string dbSolutionName, string answer, string solution, Candidate candidate)
        {
            try
            {
                General.ExecuteQuery(answer, "master");
            }
            catch (Exception e)
            {
                throw new Exception("Answer error: " + e.Message + "Querry: " + answer);
            }
            try
            {
                General.ExecuteQuery(solution, "master");
            }
            catch (Exception e)
            {
                throw new Exception("Solution error: " + e.Message + "Query: " + solution);
            }
            return CompareTwoDatabases(dbAnswerName, dbSolutionName, candidate);
        }

        /// <summary>
        /// Compare 2 databases
        /// </summary>
        /// <param name="db1Name">Student Database Name</param>
        /// <param name="db2Name">Solution Database Name</param>
        /// <returns>
        /// "true" if correct
        /// "false" if wrong
        /// message error from sqlserver if error</returns>
        private static Dictionary<string, string> CompareTwoDatabases(string db1Name, string db2Name, Candidate candidate)
        {
            try
            {
                string checkExistsSpQuery = "SELECT * FROM sys.objects\n" +
                                            "WHERE object_id = OBJECT_ID(N'[sp_CompareDb]') AND type IN ( N'P', N'PC' )";
                string compareQuery = "exec sp_CompareDb [" + db2Name + "], [" + db1Name + "]";
                var builder = Constant.SqlConnectionStringBuilder;
                // Connect to SQL
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(checkExistsSpQuery, connection))
                    {
                        //Check exists SP sp_compareDb
                        if (command.ExecuteScalar() == null)
                        {
                            try
                            {
                                General.ExecuteQuery(ProcCompareDbs, "master");
                            }
                            catch (Exception e)
                            {
                                throw new Exception("Compare error: " + e.Message);
                            }
                        }
                        using (SqlCommand commandCompare = new SqlCommand(compareQuery, connection))
                        {
                            using (SqlDataReader reader = commandCompare.ExecuteReader())
                            {
                                string result = string.Concat("Check Table structure:\n", "Table Name\t",
                                    "Column Name\t", "Data Type\t", "Nullable\n");
                                double point = candidate.Point;
                                while (reader.Read())
                                {
                                    result = string.Concat(result, (string)reader["TABLENAME"], "\t", (string)reader["COLUMNNAME"], "\t",
                                        (string)reader["DATATYPE"], "\t", (string)reader["NULLABLE"], "\n");
                                    point -= Constant.minusPoint;
                                }
                                reader.NextResult();
                                while (reader.Read())
                                {
                                    result = string.Concat(result, "Check Constraints:\n", "FK Table\t",
                                    "FK Columns\t", "PK Table\t", "PK Columns\n");
                                    result = string.Concat(result, (string)reader["FK_TABLE"], "\t", (string)reader["FK_COLUMNS"], "\t",
                                        (string)reader["PK_TABLE"], "\t", (string)reader["PK_COLUMNS"], "\n");
                                    point -= Constant.minusPoint;
                                }
                                point = (point < 0) ? 0 : point;
                                result = (point == candidate.Point) ? "True" : result;

                                return new Dictionary<string, string>()
                                                {
                                                    {"Point", point.ToString()},
                                                    {"Comment", result},
                                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Compare error: " + ex.Message);
            }

        }
    }
}

