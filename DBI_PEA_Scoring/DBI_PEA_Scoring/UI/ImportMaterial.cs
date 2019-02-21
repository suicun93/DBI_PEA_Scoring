using DBI_PEA_Scoring.Common;
using DBI_PEA_Scoring.Model;
using DBI_PEA_Scoring.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DBI_PEA_Scoring.UI
{
    public partial class ImportMaterial : Form
    {
        public string QuestionPath { get; set; }
        public Uri AnswerPath { get; set; }
        private List<ExamItem> QuestionPackage = null;
        private List<Submition> Submitions = null;

        public ImportMaterial()
        {
            InitializeComponent();
        }

        private void browseQuestionButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Get link to file
                QuestionPath = FileUtils.GetFileLocation();
                questionTextBox.Text = QuestionPath;
                // Get QuestionPackage from file
                QuestionPackage = null;
                QuestionPackage = JsonUtils.LoadQuestion(QuestionPath) as List<ExamItem>;
                if (QuestionPackage == null || QuestionPackage.Count == 0)
                    throw new Exception("No question was found!");
                else
                    MessageBox.Show("Load questions successfully", "Successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                questionTextBox.Text = "";
            }
        }

        private void browseAnswerButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Get directory where student's submittion was saved
                AnswerPath = FileUtils.GetFolderLocation();
                answerTextBox.Text = AnswerPath.LocalPath;
                // Get all submition files
                string[] SubmitionFiles = Directory.GetFiles(AnswerPath.LocalPath, "*.dat");
                if (SubmitionFiles.Count() == 0)
                    throw new Exception("No submittion was found");
                else
                    LoadSubmition(SubmitionFiles);
            }
            catch (Exception ex)
            {
                answerTextBox.Text = "";
                // Setup for status bar
                statusImportProgressBar.Maximum = 1;
                statusImportProgressBar.Value = 0;
                statusImportLabel.Text = "Imported " + "0/0";
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void LoadSubmition(string[] files)
        {
            // Init List submitions
            Submitions = new List<Submition>();
            // Setup for status bar
            int submistionCount = files.Count();
            statusImportProgressBar.Maximum = submistionCount;
            statusImportProgressBar.Step = 1;
            statusImportProgressBar.Value = 0;
            int count = 0;
            statusImportLabel.Text = "Imported " + count + "/" + submistionCount;
            // Setup for decrypt answer of student
            SecureJsonSerializer<Submition> secureJsonSerializer = new SecureJsonSerializer<Submition>();
            foreach (string file in files)
            {
                try
                {
                    Submition submition = secureJsonSerializer.Load(file);
                    Submitions.Add(submition);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                // Change value status bar
                count++;
                statusImportLabel.Text = "Imported " + count + "/" + submistionCount;
                statusImportProgressBar.Value = count;
            }
            secureJsonSerializer = null;
            MessageBox.Show("Loaded :" + Submitions.Count + " submitions.");
        }

        private void getMarkButton_Click(object sender, EventArgs e)
        {

        }

        private void ImportMaterial_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
