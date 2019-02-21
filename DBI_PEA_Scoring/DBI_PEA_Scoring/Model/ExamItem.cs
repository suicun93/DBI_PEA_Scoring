using System;
using System.Collections.Generic;

namespace DBI_PEA_Scoring.Model
{
    [Serializable]
    public class ExamItem
    {
        public ExamItem(string paperNo, List<Candidate> examQuestionsList)
        {
            PaperNo = paperNo;
            ExamQuestionsList = examQuestionsList;
        }
        public ExamItem()
        {
        }
        public String PaperNo { get; set; }
        public List<Candidate> ExamQuestionsList { get; set; }
    }
}
