using DBI_PEA_Scoring.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DBI_PEA_Scoring.UI
{
    public partial class Scoring : Form
    {

        public List<ExamItem> Questions { get; set; }
        public List<Submition> Submitions { get; set; }
        BindingSource binding = new BindingSource();
        public Scoring(List<ExamItem> questions, List<Submition> submitions)
        {

            InitializeComponent();
            Questions = questions;
            Submitions = submitions;
            this.Load += new System.EventHandler(EnumsAndComboBox_Load);
        }

        private void EnumsAndComboBox_Load(object sender, System.EventArgs e)
        {
            // Initialize the DataGridView.
            dataView.AutoGenerateColumns = false;
            //dataGridView1.AutoSize = true;
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

            // Populate the data source.
            int j = 0;
            foreach (Submition submition in Submitions)
            {
                binding.Add(submition);
                for (int i = 0; i < 10; i++)
                {
                    dataView.Rows[j].Cells[2 + i].Value = submition.ListAnswer[i];
                }
                j++;
            }
        }

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
