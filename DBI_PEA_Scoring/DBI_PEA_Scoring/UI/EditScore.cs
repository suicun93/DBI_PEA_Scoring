using DBI_PEA_Scoring.Model;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DBI_PEA_Scoring.UI
{
    public partial class EditScore : Form
    {
        public List<TestItem> ListTestItems { get; set; }
        public EditScore(List<TestItem> listTestItems)
        {
            InitializeComponent();
            ListTestItems = listTestItems;
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
                column.Name = "Diem cau " + (i + 1).ToString();
                scoreView.Columns.Add(column);
            }
            scoreView.Refresh();
            int row = 0;
            foreach (var testItem in ListTestItems)
            {
                scoreView.Rows.Add();
                scoreView.Rows[row].Cells[0].Value = testItem.PaperNo;
                int questionOrder = 0;
                foreach (var question in testItem.ExamQuestionsList)
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
