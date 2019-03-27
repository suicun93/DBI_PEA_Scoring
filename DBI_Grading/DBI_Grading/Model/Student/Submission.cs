using System;
using System.Collections.Generic;
using DBI_Grading.Common;

namespace DBI_Grading.Model.Student
{
    [Serializable]
    public class Submission
    {
        public Submission()
        {
            ListAnswer = new List<string>();
            for (var i = 0; i < Constant.PaperSet.QuestionSet.QuestionList.Count; i++)
                ListAnswer.Add("");
        }

        public Submission(string studentID, string paperNo)
        {
            StudentID = studentID;
            PaperNo = paperNo;
            ListAnswer = new List<string>();
        }

        public string StudentID { get; set; }
        public string PaperNo { get; set; }
        public List<string> ListAnswer { get; set; }

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