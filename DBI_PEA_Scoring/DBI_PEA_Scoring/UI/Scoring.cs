using DBI_PEA_Scoring.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DBI_PEA_Scoring.UI
{
    public partial class Scoring : Form
    {

        private List<BaiChungDeChamDiem> ListBaiDeCham { get; set; }
        private BindingSource binding = new BindingSource();
        public Scoring(List<ExamItem> boDe, List<Submition> submitions)
        {
            InitializeComponent();
            ListBaiDeCham = new List<BaiChungDeChamDiem>();
            SetupUI();
            this.Activated += new System.EventHandler(ChamDiem);
            // Merge Question and Submition to ListScore
            foreach (Submition submition in submitions)
            {
                BaiChungDeChamDiem baiChungDeChamDiem = new BaiChungDeChamDiem();
                // Add PaperNo
                baiChungDeChamDiem.PaperNo = submition.PaperNo;
                // Add StudentID
                baiChungDeChamDiem.StudentID = submition.StudentID;
                // Add Answers
                foreach (string answer in submition.ListAnswer)
                {
                    baiChungDeChamDiem.ListAnswers.Add(answer);
                }
                // Add Candidates
                foreach (ExamItem de in boDe)
                {
                    if (de.PaperNo.Equals(baiChungDeChamDiem.PaperNo))
                    {
                        foreach (Candidate candidate in de.ExamQuestionsList)
                        {
                            baiChungDeChamDiem.ListCandidates.Add(candidate);
                        }
                        break;
                    }
                }
                // Add to List Bai de cham
                ListBaiDeCham.Add(baiChungDeChamDiem);
            }
            this.Show();
        }

        private void SetupUI()
        {
            // Initialize the DataGridView.
            dataView.AutoGenerateColumns = false;
            dataView.DataSource = binding;

            // Initialize and add a text box column for StudentID
            DataGridViewColumn studentColumn = new DataGridViewTextBoxColumn();
            studentColumn.Name = "StudentID";
            studentColumn.DataPropertyName = "StudentID";
            dataView.Columns.Add(studentColumn);

            // Initialize and add a text box column for paperNo
            DataGridViewColumn paperNoColumn = new DataGridViewTextBoxColumn();
            paperNoColumn.Name = "PaperNo";
            paperNoColumn.DataPropertyName = "PaperNo";
            dataView.Columns.Add(paperNoColumn);

            // Initialize and add a text box column for score of each answer
            for (int i = 0; i < 10; i++)
            {
                DataGridViewColumn column = new DataGridViewTextBoxColumn();
                column.Name = "Answer " + (i + 1).ToString();
                dataView.Columns.Add(column);
            }
        }

        // Cham diem
        private void ChamDiem(object sender, EventArgs e)
        {
            // Populate the data source.
            for (int j = 0; j < ListBaiDeCham.Count; j++)
            {
                try
                {
                    BaiChungDeChamDiem baiDeCham = ListBaiDeCham.ElementAt(j);
                    baiDeCham.ChamDiem();
                    binding.Add(baiDeCham);
                    for (int i = 0; i < 10; i++)
                    {
                        dataView.Rows[j].Cells[2 + i].Value = baiDeCham.Points[i];
                    }
                    dataView.Refresh();
                }
                catch (Exception)
                {
                }
            }
        }

        // sau khi add xong thuc hien cham diem, cham den dau in diem den day!
        private void exportButton_Click(object sender, EventArgs e)
        {
            // export data to excel
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
