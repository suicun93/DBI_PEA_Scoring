using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using DBI_PEA_Scoring.Common;
using Newtonsoft.Json;

namespace DBI_PEA_Scoring.Model
{
    [Serializable]
    public class Submission
    {
        public string StudentID { get; set; }
        public string PaperNo { get; set; }
        public List<string> ListAnswer { get; set; }
        [JsonIgnore]
        public SecureJsonSerializer<Submission> secureJsonSerializer;

        public Submission()
        {
            ListAnswer = new List<string>();
            for (int i = 0; i < Constant.NumberOfQuestion; i++)
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

        public void SaveToLocal()
        {
            // When drafting answers from student, we save it to local
            try
            {
                // Write file to path ExamCode/StudentID.dat
                secureJsonSerializer.Save(this);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
