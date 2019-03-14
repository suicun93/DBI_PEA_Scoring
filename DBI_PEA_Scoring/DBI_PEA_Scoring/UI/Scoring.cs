using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DBI_PEA_Scoring.Common;
using DBI_PEA_Scoring.Model;
using DBI_PEA_Scoring.Model.Teacher;
using DBI_PEA_Scoring.Utils;

namespace DBI_PEA_Scoring.UI
{
    public partial class Scoring : Form
    {
        private List<Result> ListResults { get; set; }
        private bool scored;
        private EditScore EditScore;
        //private List<Paper> ListTestItems = null;
        private PaperSet PaperSet;
        private List<string> DBScripts;
        private int count = 0;

        public Scoring(PaperSet paperSet, List<Submission> Listsubmissions)
        {
            InitializeComponent();
            PaperSet = paperSet;
            // Show Scoring Form and generate Score here
            ListResults = new List<Result>();
            //Prepare();
            SetupUI();
            DBScripts = paperSet.DBScriptList;

            // Merge Question and submission to ListResults
            foreach (Submission submission in Listsubmissions)
            {
                Result result = new Result
                {
                    // Add PaperNo
                    PaperNo = submission.PaperNo,
                    // Add StudentID
                    StudentID = submission.StudentID
                };

                // Add Answers
                foreach (string answer in submission.ListAnswer)
                    result.ListAnswers.Add(answer);

                // Add Candidates
                foreach (Paper paper in paperSet.Papers)
                    if (paper.PaperNo.Equals(result.PaperNo))
                    {
                        foreach (Candidate candidate in paper.CandidateSet)
                            result.ListCandidates.Add(candidate);
                        break;
                    }

                // Add to List to get score
                ListResults.Add(result);
            }
            Show();
        }

        private void SetupUI()
        {
            // Initialize the DataGridView.
            scoreGridView.AutoGenerateColumns = false;
            // Initialize and add a text box column for StudentID
            scoreGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StudentID"
            });
            // Initialize and add a text box column for paperNo
            scoreGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PaperNo"
            });
            // Initialize and add a text box column for score of each answer
            for (int i = 0; i < Constant.NumberOfQuestion; i++)
                scoreGridView.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Answer " + (i + 1)
                });
        }

        /// <summary>
        /// Setup Min Max Threads in ThreadPool
        /// Show Point procedure 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowPoint(object sender, EventArgs e)
        {
            // Setup Min Max Threads in ThreadPool
            // This should be 1 because cpu is easy to run but HDD disk can not load over 2 threads in 1 time => wrong mark for student
            // So workerThreads = 1, completionPortThreads = 1;
            int workerThreads = Constant.MaxThreadPoolSize, completionPortThreads = Constant.MaxThreadPoolSize;
            ThreadPool.SetMinThreads(workerThreads: workerThreads, completionPortThreads: completionPortThreads);
            ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
            ThreadPool.SetMaxThreads(workerThreads, completionPortThreads);

            // Get Point
            if (!scored)
            {
                // Reset to count how many results has been marked.
                count = 0;
                // Populate the data source.
                for (int row = 0; row < ListResults.Count; row++)
                {
                    var CurrentResult = ListResults.ElementAt(row);
                    // Prepare 2 first columns
                    scoreGridView.Invoke((MethodInvoker)(() =>
                    {
                        scoreGridView.Rows.Add(1);
                        scoreGridView.Rows[row].Cells[0].Value = CurrentResult.StudentID;
                        scoreGridView.Rows[row].Cells[1].Value = CurrentResult.PaperNo;
                    }));
                    Input input = new Input(row, CurrentResult);
                    ThreadPool.QueueUserWorkItem(dontKnowHowItWork => GetPoint(input));
                }
                scored = true;
            }
            else MessageBox.Show("Score has already got.");
        }

        private void GetPoint(Input input)
        {
            input.Result.GetPoint();
            // Refresh to show point and scroll view to the last row
            // Show point of each question
            scoreGridView.Invoke((MethodInvoker)(() =>
            {
                for (int questionOrder = 0; questionOrder < input.Result.ListCandidates.Count; questionOrder++)
                {
                    scoreGridView.Rows[input.Row].Cells[2 + questionOrder].Value = input.Result.Points[questionOrder].ToString();
                }
            }));
            CountDown();
        }

        /// <summary>
        /// Handle ThreadPool Completion
        /// </summary>
        private void CountDown()
        {
            count++;
            if (count == ListResults.Count)
            {
                // Done
                MessageBox.Show("Done");
                editScoreButton.Invoke((MethodInvoker)(() => { editScoreButton.Enabled = true; }));

                exportButton.Invoke((MethodInvoker)(() => { exportButton.Enabled = true; })); 
                startButton.Invoke((MethodInvoker)(() => { startButton.Visible = false; }));
            }
        }

        // sau khi add xong thuc hien cham diem, cham den dau in diem den day!
        private void ExportButton_Click(object sender, EventArgs e)
        {
            try
            {
                double maxPoint = 0;
                foreach (Candidate candidate in ListResults.ElementAt(0).ListCandidates)
                    maxPoint += candidate.Point;
                ExcelUtils.ExportResultsExcel(ListResults, maxPoint, ListResults.ElementAt(0).ListCandidates.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void Scoring_FormClosed(object sender, FormClosedEventArgs e) => Application.Exit();

        private void QuitButton_Click(object sender, EventArgs e) => Application.Exit();

        private void EditScoreButton_Click(object sender, EventArgs e)
        {
            // Edit lai bieu diem cho cac cau hoi
            EditScore = new EditScore(PaperSet);
            EditScore.Show();
        }
    }
}
