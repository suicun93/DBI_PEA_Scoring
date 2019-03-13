using System;
using System.Windows.Forms;
using DBI_PEA_Scoring.Model.Teacher;

namespace DBI_PEA_Scoring.UI
{
    public partial class EditScore : Form
    {
        public PaperSet PaperSet { get; set; }
        public EditScore(PaperSet paperSet)
        {
            InitializeComponent();
            PaperSet = paperSet;
            SetupUI();
        }

        private void SetupUI()
        {
            // Initialize the DataGridView.
            scoreView.AutoGenerateColumns = false;

            // Initialize and add a text box column for StudentID
            DataGridViewColumn TeskPackageColumn = new DataGridViewTextBoxColumn();
            TeskPackageColumn.Name = "Ma de";
            scoreView.Columns.Add(TeskPackageColumn);

            // Initialize and add a text box column for score of each answer
            for (int i = 0; i < 10; i++)
            {
                DataGridViewColumn column = new DataGridViewTextBoxColumn();
                column.Name = "Diem cau " + (i + 1);
                scoreView.Columns.Add(column);
            }
            scoreView.Refresh();
            int row = 0;
            foreach (var paper in PaperSet.Papers)
            {
                scoreView.Rows.Add();
                scoreView.Rows[row].Cells[0].Value = paper.PaperNo;
                int questionOrder = 0;
                foreach (var question in paper.CandidateSet)
                {
                    questionOrder++;
                    scoreView.Rows[row].Cells[questionOrder].Value = question.Point.ToString();
                }
                row++;
            }
            scoreView.Refresh();
        }
        private void saveButton_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void EditScore_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveButton_Click(sender, e);
        }
    }
}
