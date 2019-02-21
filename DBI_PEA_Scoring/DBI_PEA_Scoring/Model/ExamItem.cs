using System;
using System.Collections.Generic;

namespace DBI_PEA_Scoring.Model
{
    [Serializable]
    class ExamItem
    {
        public ExamItem(string examItemCode, List<Candidate> examQuestionsList)
        {
            ExamItemCode = examItemCode;
            ExamQuestionsList = examQuestionsList;
        }
        public ExamItem()
        {
        }
        public String ExamItemCode { get; set; }
        public List<Candidate> ExamQuestionsList { get; set; }
    }
}
