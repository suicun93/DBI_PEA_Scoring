using System;
using System.Collections.Generic;

namespace DBI_PEA_Scoring.Model
{
    [Serializable]
    public class TestItem
    {
        public string PaperNo { get; set; }
        public List<Candidate> ExamQuestionsList { get; set; }
        public string ExamCode { get; set; }
    }
}
