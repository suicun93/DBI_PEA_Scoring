using System;
using System.Windows.Forms;
using DBI_PEA_Scoring.Common;
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
            scoreView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Ma de"
            });

            // Initialize and add a text box column for score of each answer
            for (int i = 0; i < Constant.NumberOfQuestion; i++)
            {
                scoreView.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Name = "Diem cau " + (i + 1)
                });
            }
            int row = 0;
            foreach (Paper paper in PaperSet.Papers)
            {
                scoreView.Rows.Add();
                scoreView.Rows[row].Cells[0].Value = paper.PaperNo;
                scoreView.Invoke((MethodInvoker)(() =>
                {
                    for (int questionOrder = 0; questionOrder < paper.CandidateSet.Count; questionOrder++)
                        scoreView.Rows[row].Cells[questionOrder].Value = paper.CandidateSet[questionOrder].Point.ToString();
                }));
                row++;
            }
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void EditScore_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveButton_Click(sender, e);
        }
    }
}
