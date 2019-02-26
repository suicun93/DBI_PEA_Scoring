using DBI_PEA_Scoring.Common;
using System;
using System.Data.SqlClient;

namespace DBI_PEA_Scoring.Utils.DaoType
{
    partial class SchemaType
    {
        /// <summary>
        /// Mark Student query
        /// </summary>
        /// <param name="dbTeacherName"></param>
        /// <param name="dbStudentName"></param>
        /// <param name="queryTeacher">query from Teacher</param>
        /// <param name="queryStudent">query from Student</param>
        /// <returns>
        /// "true" if correct
        /// "false" if wrong
        /// message error from sqlserver if error</returns>
        public static bool MarkSchemaDatabasesType(string dbTeacherName, string dbStudentName, string queryTeacher,
            string queryStudent)
        {
            var builder = Constant.SqlConnectionStringBuilder;
            string createDbTeacherQuery = "CREATE DATABASE " + dbTeacherName + "";
            string createDbStudentQuery = "CREATE DATABASE " + dbStudentName + "";
            queryTeacher = "USE " + dbTeacherName + "\n" + queryTeacher + "\n";
            queryStudent = "USE " + dbStudentName + "\n" + queryStudent + "\n";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                using (SqlCommand commandCreateDatabase = new SqlCommand(createDbStudentQuery, connection))
                {
                    commandCreateDatabase.ExecuteNonQuery();
                }
                using (SqlCommand commandCreateDatabase = new SqlCommand(createDbTeacherQuery, connection))
                {
                    commandCreateDatabase.ExecuteNonQuery();
                }
                using (SqlCommand commandTeacher = new SqlCommand(queryStudent, connection))
                {
                    commandTeacher.ExecuteNonQuery();
                }
                using (SqlCommand commandStudent = new SqlCommand(queryTeacher, connection))
                {
                    commandStudent.ExecuteNonQuery();
                }
                return CompareTwoDatabases(dbStudentName, dbTeacherName);
            }
        }

        /// <summary>
        /// Compare 2 databases
        /// </summary>
        /// <param name="db1Name"></param>
        /// <param name="db2Name"></param>
        /// <returns>
        /// "true" if correct
        /// "false" if wrong
        /// message error from sqlserver if error</returns>
        private static bool CompareTwoDatabases(string db1Name, string db2Name)
        {
            string checkExistsSpQuery = "USE master\n" +
                                        "SELECT * FROM sys.objects\n" +
                                        "WHERE object_id = OBJECT_ID(N'[sp_CompareDb]') AND type IN ( N'P', N'PC' )";
            string compareQuery = "exec sp_CompareDb [" + db1Name + "], [" + db2Name + "]";
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
                        //Create SP
                        using (SqlCommand commandCreate = new SqlCommand(ProcCompareDbs, connection))
                        {
                            commandCreate.ExecuteNonQuery();
                        }
                    }
                    using (SqlCommand commandCompare = new SqlCommand(compareQuery, connection))
                    {
                        using (SqlDataReader reader = commandCompare.ExecuteReader())
                        {
                            do
                            {
                                while (reader.HasRows)
                                {
                                    return false;
                                }
                            } while (reader.NextResult());
                            return true;
                        }
                    }
                }
            }
        }
    }
}

