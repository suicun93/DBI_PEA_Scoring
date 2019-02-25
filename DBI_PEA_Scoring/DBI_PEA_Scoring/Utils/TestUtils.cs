using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using DBI_PEA_Scoring.Model;
using DBI_PEA_Scoring.Utils.DaoType;
using DBI_PEA_Scoring.Common;

namespace DBI_PEA_Scoring.Utils
{
    public class TestUtils
    {
        internal static bool TestQuery(Candidate candidate, string answer)
        {
            // Build connection string
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = Constant.listDB[0].DataSource,
                UserID = Constant.listDB[0].UserId,
                Password = Constant.listDB[0].Password,
                InitialCatalog = Constant.listDB[0].InitialCatalog
            };
            SelectType st = new SelectType(builder);
            //Duplicate 2 new DB for student and teacher
            General.DuplicatedDb(builder, Constant.listDB[0].SqlServerDbFolder,
                Constant.listDB[0].SourceDbName, "DbForTest");
            string dbTeacherName = "DbForTest_Teacher";
            string dbStudentName = "DbForTest_Student";
            try
            {
                //Marking
                foreach (Requirement req in candidate.Requirements)
                {
                    // In case question type is Query, requirement type default is result set. 
                    if (st.MarkQueryType(req.RequireSort, dbTeacherName, dbStudentName,
                            req.ResultQuery, answer, builder) == false)
                    {
                        General.DropDatabase(dbTeacherName, builder);
                        General.DropDatabase(dbStudentName, builder);
                        return false;
                    }
                }
                General.DropDatabase(dbTeacherName, builder);
                General.DropDatabase(dbStudentName, builder);
                return true;
            }
            catch (SqlException e)
            {
                General.DropDatabase(dbTeacherName, builder);
                General.DropDatabase(dbStudentName, builder);
                throw e;
            }
        }

        internal static bool TestSchema(Candidate candidate, string answer)
        {
            // Build connection string
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = Constant.listDB[0].DataSource,
                UserID = Constant.listDB[0].UserId,
                Password = Constant.listDB[0].Password,
                InitialCatalog = Constant.listDB[0].InitialCatalog
            };
            SchemaType st = new SchemaType(builder);

            string dbTeacherName = "DbForTest_Teacher";
            string dbStudentName = "DbForTest_Student";

            try
            {
                //Marking
                foreach (Requirement req in candidate.Requirements)
                {
                    if (st.MarkSchemaDatabasesType(dbTeacherName, dbStudentName,
                        answer, req.ResultQuery) == false)
                    {
                        return false;
                    }
                }
                General.DropDatabase(dbTeacherName, builder);
                General.DropDatabase(dbStudentName, builder);
                return true;
            }
            catch (SqlException e)
            {
                General.DropDatabase(dbTeacherName, builder);
                General.DropDatabase(dbStudentName, builder);
                throw e;
            }
        }

        internal static bool TestInsertDeleteUpdate(Candidate candidate, string answer)
        {
            // Build connection string
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = Constant.listDB[0].DataSource,
                UserID = Constant.listDB[0].UserId,
                Password = Constant.listDB[0].Password,
                InitialCatalog = Constant.listDB[0].InitialCatalog
            };
            SelectType st = new SelectType(builder);
            string dbTeacherName = "DbForTest_Teacher";
            string dbStudentName = "DbForTest_Student";
            bool noRequireSort = false;
            try
            {
                // In case question type is InsertDeleteUpdate, requirement type default is Effect. 
                //Marking
                foreach (Requirement req in candidate.Requirements)
                {
                    // Execute query
                    General.ExecuteQuery(dbTeacherName, dbStudentName, req.ActivateTriggerQuery, answer, builder);
                    if (st.MarkQueryType(noRequireSort, dbTeacherName, dbStudentName,
                             req.ResultQuery, req.ResultQuery, builder) == false)
                    {
                        General.DropDatabase(dbTeacherName, builder);
                        General.DropDatabase(dbStudentName, builder);
                        return false;
                    }
                }
                General.DropDatabase(dbTeacherName, builder);
                General.DropDatabase(dbStudentName, builder);
                return true;
            }
            catch (SqlException e)
            {
                General.DropDatabase(dbTeacherName, builder);
                General.DropDatabase(dbStudentName, builder);
                throw e;
            }
        }

        internal static bool TestProcedure(Candidate candidate, string answer)
        {
            throw new NotImplementedException();
        }
        internal static bool TestTrigger(Candidate candidate, string answer)
        {
            throw new NotImplementedException();
        }
    }
}
