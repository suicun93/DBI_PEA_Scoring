using DBI_PEA_Scoring.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace DBI_PEA_Scoring.Model
{
    [Serializable]
    public class Submition
    {
        public string StudentID { get; set; }
        public string ExamCode { get; set; }
        public string PaperNo { get; set; }
        public List<string> ListAnswer { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public SecureJsonSerializer<Submition> secureJsonSerializer;

        public Submition()
        {
        }

        public Submition(string examCode, string studentID, string paperNo)
        {
            ExamCode = examCode;
            StudentID = studentID;
            PaperNo = paperNo;
            ListAnswer = new List<string>();
        }

        public void register()
        {
            var dir = ExamCode;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            secureJsonSerializer = new SecureJsonSerializer<Submition>(Path.Combine(dir, StudentID + ".dat"));
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
                    Submition submition;
                    try
                    {
                        submition = secureJsonSerializer.Load();
                        // Load successfully
                        ListAnswer = new List<string>();
                        foreach (var answer in submition.ListAnswer)
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
                            this.ListAnswer.Add("");
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
