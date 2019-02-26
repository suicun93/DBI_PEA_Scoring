using DBI_PEA_Scoring.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DBI_PEA_Scoring.Model
{
    class Result
    {
        public string StudentID { get; set; }
        public string PaperNo { get; set; }
        public List<string> ListAnswers { get; set; }
        public List<Candidate> ListCandidates { get; set; }
        public double[] Points { get; set; }
        public string[] Logs { get; set; }

        public Result()
        {
            Points = new double[10];
            ListAnswers = new List<string>();
            ListCandidates = new List<Candidate>();
            Logs = new string[10];
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
        private bool Point(Candidate candidate, string answer)
        {
            // await TaskEx.Delay(100);
            if (string.IsNullOrEmpty(answer))
                return false;
            // Process by Question Type
            switch (candidate.QuestionType)
            {
                case Candidate.QuestionTypes.Schema:
                    // Schema Question
                    return TestUtils.TestSchema(candidate, answer);
                case Candidate.QuestionTypes.DML:
                    // DML: Insert/Delete/Update Question
                    return TestUtils.TestDML(candidate, answer);
                case Candidate.QuestionTypes.Select:
                    //Select Question
                    return TestUtils.TestSelect(candidate, answer);
                case Candidate.QuestionTypes.Procedure:
                    // Procedure Question
                    return TestUtils.TestProcedure(candidate, answer);
                case Candidate.QuestionTypes.Trigger:
                    // Trigger Question
                    return TestUtils.TestTrigger(candidate, answer);
                    
                    
                default:
                    // Not supported yet
                    throw new System.Exception("This question type have not been supported");
            }
        }

        /// <summary>
        /// Get Point function
        /// </summary>
        /// <param name="dataGridView"> Data Grid View to show point of student</param>
        /// <param name="row">The row number where the student is</param>
        public void GetPoint(DataGridView dataGridView, int row)
        {
            // Count number of candidate
            int numberOfQuestion = ListCandidates.Count;
            // Prepare 2 first columns
            dataGridView.Rows.Add(1);
            dataGridView.Rows[row].Cells[0].Value = StudentID;
            dataGridView.Rows[row].Cells[1].Value = PaperNo;
            for (int questionOrder = 0; questionOrder < 10; questionOrder++)
            {
                try
                {
                    //bool correct = await Cham(ListCandidates.ElementAt(i), ListAnswers.ElementAt(i));
                    if (numberOfQuestion > questionOrder)
                        if (Point(ListCandidates.ElementAt(questionOrder), ListAnswers.ElementAt(questionOrder)))
                        {
                            // Exactly -> Log true and return 0 point
                            Points[questionOrder] = ListCandidates.ElementAt(questionOrder).Point;
                            Logs[questionOrder] = "";
                        }
                        else
                        {
                            // Wrong -> Log false and return 0 point
                            Points[questionOrder] = 0;
                            Logs[questionOrder] = "false";
                        }
                    else
                    {
                        // Not enough candidate 
                        // It rarely happens, it's this project's demos and faults.
                        Points[questionOrder] = 0;
                        Logs[questionOrder] = "No questions found at question " + questionOrder + " paperNo = " + PaperNo;
                    }
                }
                catch (System.Exception e)
                {
                    // When something's wrong:
                    // Log error and return 0 point for student.
                    Points[questionOrder] = 0;
                    Logs[questionOrder] = e.Message;
                }
                // Show point of each question
                dataGridView.Rows[row].Cells[2 + questionOrder].Value = Points[questionOrder].ToString();
            }
            // Refresh to show point and scroll view to the last row
            dataGridView.Refresh();
            dataGridView.FirstDisplayedScrollingRowIndex = dataGridView.RowCount - 1;
        }
    }
}
