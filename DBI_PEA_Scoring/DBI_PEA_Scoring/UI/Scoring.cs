using System;
using System.Collections.Generic;
using System.IO;
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

        public Scoring(PaperSet paperSet, List<Submission> Listsubmissions)
        {
            InitializeComponent();
            PaperSet = paperSet;
            // Show Scoring Form and generate Score here
            ListResults = new List<Result>();
            //Prepare();
            SetupUI();
            DBScripts = paperSet.DBScriptList;

            //this.Activated += new System.EventHandler(ShowPoint);
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
                {
                    result.ListAnswers.Add(answer);
                }
                // Add Candidates
                foreach (Paper paper in paperSet.Papers)
                {
                    if (paper.PaperNo.Equals(result.PaperNo))
                    {
                        foreach (Candidate candidate in paper.CandidateSet)
                        {
                            result.ListCandidates.Add(candidate);
                        }
                        break;
                    }
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
            DataGridViewColumn studentColumn = new DataGridViewTextBoxColumn();
            studentColumn.Name = "StudentID";
            scoreGridView.Columns.Add(studentColumn);

            // Initialize and add a text box column for paperNo
            DataGridViewColumn paperNoColumn = new DataGridViewTextBoxColumn();
            paperNoColumn.Name = "PaperNo";
            scoreGridView.Columns.Add(paperNoColumn);

            // Initialize and add a text box column for score of each answer
            for (int i = 0; i < Constant.NumberOfQuestion; i++)
            {
                DataGridViewColumn column = new DataGridViewTextBoxColumn();
                column.Name = "Answer " + (i + 1);
                scoreGridView.Columns.Add(column);
            }
        }

        // Prepare environment
        private void Prepare()
        {
            //General.ExecuteSingleQuery("use master drop proc sp_CompareDb");
            var dir = "C:\\Temp\\";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        // Cham diem
        private void ShowPoint(object sender, EventArgs e)
        {
            int a =1, b = 1;
            if (ThreadPool.SetMinThreads(a, b))
            {
                MessageBox.Show("a= " + a + " b= " + b);
            }
            ThreadPool.GetMinThreads(out a, out b); //a=1, b=1 on my machine
            if (ThreadPool.SetMaxThreads(a, b))
            {
                MessageBox.Show("a= " + a + " b= " + b);
            }
            if (!scored)
            {
                // Populate the data source.
                for (int row = 0; row < ListResults.Count; row++)
                {
                    var CurrentResult = ListResults.ElementAt(row);
                    // Prepare 2 first columns
                    scoreGridView.Rows.Add(1);
                    scoreGridView.Rows[row].Cells[0].Value = CurrentResult.StudentID;
                    scoreGridView.Rows[row].Cells[1].Value = CurrentResult.PaperNo;
                    scoreGridView.Refresh();
                    Input input = new Input(scoreGridView, row, CurrentResult);
                    ThreadPool.QueueUserWorkItem(KetPoint, input);
                }
                //scoreGridView.Refresh();
                scored = true;
            }
            else MessageBox.Show("Score has already got.");
        }
        private void KetPoint(object input)
        {
            var temp = input as Input;
            temp.Result.GetPoint();
            // Refresh to show point and scroll view to the last row
            // Show point of each question
            for (int questionOrder = 0; questionOrder < temp.Result.ListCandidates.Count; questionOrder++)
            {
                scoreGridView.Invoke(new MethodInvoker(() =>
                {
                    scoreGridView.Rows[temp.Row].Cells[2 + questionOrder].Value = temp.Result.Points[questionOrder].ToString();
                    scoreGridView.UpdateCellValue(2 + questionOrder, temp.Row);
                }));
            }
        }
        //delegate void SetTextCallback(Result result);
        // sau khi add xong thuc hien cham diem, cham den dau in diem den day!
        private void ExportButton_Click(object sender, EventArgs e)
        {
            try
            {
                double maxPoint = 0;
                foreach (Candidate candidate in ListResults.ElementAt(0).ListCandidates)
                {
                    maxPoint += candidate.Point;
                }
                ExcelUtils.ExportResultsExcel(ListResults, maxPoint, ListResults.ElementAt(0).ListCandidates.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void Scoring_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void EditScoreButton_Click(object sender, EventArgs e)
        {
            // Edit lai bieu diem cho cac cau hoi
            EditScore = new EditScore(PaperSet);
            EditScore.Show();
        }
    }
}
