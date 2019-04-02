using System;
using System.Collections.Generic;

namespace DBI_Grading.Model.Teacher
{
    [Serializable]
    public class QuestionSet
    {
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

        public List<Question> QuestionList { get; set; }
        public List<string> DBScriptList { get; set; }
    }
}