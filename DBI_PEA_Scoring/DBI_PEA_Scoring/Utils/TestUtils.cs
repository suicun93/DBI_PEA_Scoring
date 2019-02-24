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
            General gen = new General();
            //Duplicate 2 new DB for student and teacher
            gen.DuplicatedDb(builder, Constant.listDB[0].SqlServerDbFolder, 
                Constant.listDB[0].SourceDbName, "DbForTest");
            string dbNameTeacher = "DbForTest_Teacher";
            string dbNameStudent = "DbForTest_Student";
            //Marking
            foreach (Requirement req in candidate.Requirements)
            {
                if (st.MarkQueryType(req.RequireSort, dbNameTeacher, dbNameStudent,
                        req.ResultQuery, answer, builder) == false)
                {
                    return false;
                };
            }
            return true;
        }

        internal static bool TestProcedure(Candidate candidate, string answer)
        {
            throw new NotImplementedException();
        }

        internal static bool TestScheme(Candidate candidate, string answer)
        {
            throw new NotImplementedException();
        }
    }
}
