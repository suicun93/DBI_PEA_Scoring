using DBI_PEA_Scoring.Common;
using DBI_PEA_Scoring.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace DBI_PEA_Scoring.Model
{
    public class Result
    {
        public string StudentID { get; set; }
        public string PaperNo { get; set; }
        public string ExamCode { get; set; }
        public List<string> ListAnswers { get; set; }
        public List<Candidate> ListCandidates { get; set; }
        public double[] Points { get; set; }
        public string[] Logs { get; set; }

        public Result()
        {
            Points = new double[Constant.NumberOfQuestion];
            ListAnswers = new List<string>();
            ListCandidates = new List<Candidate>();
            Logs = new string[Constant.NumberOfQuestion];
        }

        /// <summary>
        ///  Get Sum of point
        /// </summary>
        /// <returns> Sum of point(double)</returns>
        public double SumOfPoint()
        {
            double sum = 0;
            foreach (double point in Points)
                sum += point;
            return sum;
        }

        /// <summary>
        /// Count Student's point by question and answer
        /// </summary>
        /// <param name="candidate">Question</param>
        /// <param name="answer">Student's answer</param>
        /// <returns>
        ///     True if correct
        ///     False if incorrect.
        /// </returns>
        /// <exception cref="SQLException">
        ///     if exception was found, throw it for GetPoint function to handle
        /// </exception>
        private Dictionary<string, string> Point(Candidate candidate, string answer, int questionOrder)
        {
            // await TaskEx.Delay(100);
            if (string.IsNullOrEmpty(answer))
                throw new Exception("Empty.");
            // Process by Question Type
            switch (candidate.QuestionType)
            {
                case Candidate.QuestionTypes.Schema:
                    // Schema Question
                    return TestUtils.TestSchema(candidate, StudentID, answer, questionOrder);
                case Candidate.QuestionTypes.Select:
                    //Select Question
                    return TestUtils.TestSelect(candidate, StudentID, answer, questionOrder);
                case Candidate.QuestionTypes.DML:
                    // DML: Insert/Delete/Update Question
                    return TestUtils.TestInsertDeleteUpdate(candidate, StudentID, answer, questionOrder);
                case Candidate.QuestionTypes.Procedure:
                    // Procedure Question
                    return TestUtils.TestProcedure(candidate, StudentID, answer, questionOrder);
                case Candidate.QuestionTypes.Trigger:
                    // Trigger Question
                    return TestUtils.TestTrigger(candidate, StudentID, answer, questionOrder);
                default:
                    // Not supported yet
                    throw new Exception("This question type has not been supported yet.");
            }
        }

        /// <summary>
        /// Get Point function
        /// </summary>
        /// <param name="dataGridView"> Data Grid View to show point of student</param>
        /// <param name="row">The row number where the student is</param>
        public void GetPoint()
        {
            // Count number of candidate
            int numberOfQuestion = ListCandidates.Count;
            // Wrong PaperNo
            if (numberOfQuestion == 0)
            {
                Logs[0] = "Wrong Paper No\n";
                for (int i = 0; i < Constant.NumberOfQuestion; i++)
                {
                    Points[i] = 0;
                }
                return;
            }
            //// Select random DB
            //TestUtils.Database = Constant.listDB[(new Random()).Next(1, Common.Constant.listDB.Length) - 1];
            // Get mark one by one
            for (int questionOrder = 0; questionOrder < 10; questionOrder++)
            {
                try
                {
                    //bool correct = await Cham(ListCandidates.ElementAt(i), ListAnswers.ElementAt(i));
                    if (numberOfQuestion > questionOrder)
                    {
                        Dictionary<string, string> res = Point(ListCandidates.ElementAt(questionOrder), ListAnswers.ElementAt(questionOrder), questionOrder);
                        //Exactly -> Log true and return 0 point
                        if(res != null)
                        {
                            Points[questionOrder] = double.Parse(res["Point"]);
                            Logs[questionOrder] = res["Comment"] + "\n";
                        }
                        else
                        {
                            Points[questionOrder] = 0;
                            Logs[questionOrder] = "False\n";
                        }
                    }
                    else
                    {
                        // Not enough candidate 
                        // It rarely happens, it's this project's demos and faults.
                        throw new Exception("No questions found at question " + questionOrder + " paperNo = " + PaperNo + "\n");
                    }
                }
                catch (Exception e)
                {
                    // When something's wrong:
                    // Log error and return 0 point for student.
                    Points[questionOrder] = 0;
                    Logs[questionOrder] = e.Message + "\n";
                }
            }
        }
    }
}