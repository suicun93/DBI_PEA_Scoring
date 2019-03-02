using DBI_PEA_Scoring.Model;
using DBI_PEA_Scoring.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DBI_PEA_Scoring.UI
{
    public partial class Scoring : Form
    {

        private List<Result> ListResults { get; set; }
        private bool scored = false;
        public Scoring(List<TestItem> ListExamItems, List<Submition> ListSubmitions)
        {
            InitializeComponent();
            ListResults = new List<Result>();
            SetupUI();
            //this.Activated += new System.EventHandler(ShowPoint);
            // Merge Question and Submition to ListResults
            foreach (Submition submition in ListSubmitions)
            {
                Result result = new Result();
                // Add PaperNo
                result.PaperNo = submition.PaperNo;
                // Add StudentID
                result.StudentID = submition.StudentID;
                // Add Answers
                foreach (string answer in submition.ListAnswer)
                {
                    result.ListAnswers.Add(answer);
                }
                // Add Candidates
                foreach (TestItem de in ListExamItems)
                {
                    if (de.PaperNo.Equals(result.PaperNo))
                    {
                        foreach (Candidate candidate in de.ExamQuestionsList)
                        {
                            result.ListCandidates.Add(candidate);
                        }
                        break;
                    }
                }
                // Add to List to get score
                ListResults.Add(result);
            }
            this.Show();
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
            for (int i = 0; i < 10; i++)
            {
                DataGridViewColumn column = new DataGridViewTextBoxColumn();
                column.Name = "Answer " + (i + 1).ToString();
                scoreGridView.Columns.Add(column);
            }
        }

        // Cham diem
        private void ShowPoint(object sender, EventArgs e)
        {
            if (!scored)
            {
                // Populate the data source.
                for (int row = 0; row < ListResults.Count; row++)
                {
                    ListResults.ElementAt(row).GetPoint(scoreGridView, row);
                }
                scored = true;
                // Delete origin database
                Utils.DaoType.General.ClearDatabase();
            }
            else MessageBox.Show("Score has already got.");
        }

        // sau khi add xong thuc hien cham diem, cham den dau in diem den day!
        private void exportButton_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1.Filter = "Microsoft Excel 97-2003 Add-In (*.doc)|*.doc";
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string saveFolder = Path.GetDirectoryName(saveFileDialog1.FileName);
                    string savePath = Path.Combine(saveFolder, saveFileDialog1.FileName);
                    double maxPoint = 0;
                    foreach (Candidate candidate in ListResults.ElementAt(0).ListCandidates)
                    {
                        maxPoint += candidate.Point;
                    }
                    ExcelUtils.ExportResultsExcel(savePath, ListResults, maxPoint);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error");
            }
           
        }

        private void Scoring_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void quitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
