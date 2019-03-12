using System;
using System.Collections.Generic;

namespace DBI_PEA_Scoring.Model.Teacher
{
    [Serializable]
    public class PaperSet
    {
        public List<Paper> Papers { get; set; }
        public List<string> DBScriptList { get; set; }

        public PaperSet(List<Paper> papers, List<string> dbScriptList)
        {
            Papers = papers;
            DBScriptList = dbScriptList;
        }

        public PaperSet() { }
    }
}
