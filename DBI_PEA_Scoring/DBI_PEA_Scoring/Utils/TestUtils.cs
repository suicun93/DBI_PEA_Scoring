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
        internal static bool TestTrigger(Candidate candidate, string answer)
        {
            throw new NotImplementedException();
        }

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
            General general = new General();
            //Duplicate 2 new DB for student and teacher
            general.DuplicatedDb(builder, Constant.listDB[0].SqlServerDbFolder,
                Constant.listDB[0].SourceDbName, "DbForTest");
            string dbTeacherName = "DbForTest_Teacher";
            string dbStudentName = "DbForTest_Student";
            try
            {
                //Marking
                foreach (Requirement req in candidate.Requirements)
                {
                    if (st.MarkQueryType(req.RequireSort, dbTeacherName, dbStudentName,
                            req.ResultQuery, answer, builder) == false)
                    {
                        general.DropDatabase(dbTeacherName, builder);
                        general.DropDatabase(dbStudentName, builder);
                        return false;
                    }
                }
                general.DropDatabase(dbTeacherName, builder);
                general.DropDatabase(dbStudentName, builder);
                return true;
            }
            catch (SqlException e)
            {
                general.DropDatabase(dbTeacherName, builder);
                general.DropDatabase(dbStudentName, builder);
                throw e;
            }
        }

        internal static bool TestProcedure(Candidate candidate, string answer)
        {
            throw new NotImplementedException();
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
            General general = new General();
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
                general.DropDatabase(dbTeacherName, builder);
                general.DropDatabase(dbStudentName, builder);
                return true;
            }
            catch (SqlException e)
            {
                general.DropDatabase(dbTeacherName, builder);
                general.DropDatabase(dbStudentName, builder);
                throw e;
            }
        }
    }
}
