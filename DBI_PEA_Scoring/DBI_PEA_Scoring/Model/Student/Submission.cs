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
        public string ExamCode { get; set; }
        public string PaperNo { get; set; }
        public List<string> ListAnswer { get; set; }
        [JsonIgnore]
        public SecureJsonSerializer<Submission> secureJsonSerializer;

        public Submission()
        {
        }

        public Submission(string examCode, string studentID, string paperNo)
        {
            ExamCode = examCode;
            StudentID = studentID;
            PaperNo = paperNo;
            ListAnswer = new List<string>();
        }

        public void Register()
        {
            var dir = ExamCode;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            secureJsonSerializer = new SecureJsonSerializer<Submission>(Path.Combine(dir, StudentID + ".dat"));
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

        // Restore when students continue doing their exam.
        public void Restore()
        {
            try
            {
                var dir = ExamCode;
                if (Directory.Exists(dir))
                {
                    Submission submission;
                    try
                    {
                        submission = secureJsonSerializer.Load();
                        // Load successfully
                        ListAnswer = new List<string>();
                        foreach (var answer in submission.ListAnswer)
                        {
                            ListAnswer.Add(answer);
                        }
                    }
                    catch (Exception)
                    {
                        // Load fail
                        MessageBox.Show("Restore fail");
                        // Create new list
                        for (int i = 0; i < 10; i++)
                        {
                            ListAnswer.Add("");
                        }
                    }
                }
                else
                {
                    throw new Exception("No file was found");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
