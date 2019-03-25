using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBI_PEA_Scoring.Model.Teacher
{
    public class QuestionSet
    {
        public List<Question> QuestionList { get; set; }
        public List<string> DBScriptList { get; set; }

        public QuestionSet()
        {
            QuestionList = new List<Question>();
            DBScriptList = new List<string>();
        }

        public QuestionSet(List<Question> questionList, List<string> dBScriptList)
        {
            QuestionList = questionList;
            DBScriptList = dBScriptList;
        }
    }
}
