using System;
using System.Collections.Generic;
using DBI_PEA_Scoring.Common;

namespace DBI_PEA_Scoring.Model
{
    [Serializable]
    public class Submission
    {
        public string StudentID { get; set; }
        public string PaperNo { get; set; }
        public List<string> ListAnswer { get; set; }

        public Submission()
        {
            ListAnswer = new List<string>();
            for (int i = 0; i < Constant.PaperSet.QuestionSet.QuestionList.Count; i++)
            {
                ListAnswer.Add("");
            }
        }

        public Submission(string studentID, string paperNo)
        {
            StudentID = studentID;
            PaperNo = paperNo;
            ListAnswer = new List<string>();
        }

        public void AddAnswer(string answer)
        {
            ListAnswer.Add(answer);
        }

        public void ClearAnswer()
        {
            ListAnswer.Clear();
        }
    }
}
