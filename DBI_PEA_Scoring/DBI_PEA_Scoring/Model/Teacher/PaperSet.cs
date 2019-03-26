using System;
using System.Collections.Generic;

namespace DBI_PEA_Grading.Model.Teacher
{
    [Serializable]
    public class PaperSet
    {
        public PaperSet(List<Paper> papers, List<string> dbScriptList)
        {
            Papers = papers;
            DBScriptList = dbScriptList;
        }

        public PaperSet()
        {
        }

        public List<Paper> Papers { get; set; }
        public List<string> DBScriptList { get; set; }
        public List<int> ListPaperMatrixId { get; set; }
        public QuestionSet QuestionSet { get; set; }
    }
}