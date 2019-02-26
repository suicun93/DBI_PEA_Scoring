using DBI_PEA_Scoring.Common;
using DBI_PEA_Scoring.Model;
using DBI_PEA_Scoring.Utils;
using DBI_PEA_Scoring.Utils.DaoType;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
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
        public string DBPath { get; set; }
        public string AnswerPath { get; set; }
        private List<ExamItem> ListExamItems = null;
        private List<Submition> ListSubmitions = null;
        private bool importedDB = false;
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
                ListExamItems = null;
                ListExamItems = JsonUtils.LoadQuestion(QuestionPath) as List<ExamItem>;
                if (ListExamItems == null || ListExamItems.Count == 0)
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
                answerTextBox.Text = AnswerPath;
                // Get all submition files
                string[] SubmitionFiles = Directory.GetFiles(AnswerPath, "*.dat");
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
            ListSubmitions = new List<Submition>();
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
                    ListSubmitions.Add(submition);
                    // Change value status bar
                    count++;
                    statusImportLabel.Text = "Imported " + count + "/" + submistionCount;
                    statusImportProgressBar.Value = count;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            secureJsonSerializer = null;
            // After loading successfully, diable browse button because jsonSerialize is error if load again.
            browseAnswerButton.Enabled = false;
            MessageBox.Show("Loaded :" + ListSubmitions.Count + " submitions.");
        }

        private void getMarkButton_Click(object sender, EventArgs e)
        {
            if (ListSubmitions == null || ListExamItems == null || ListSubmitions.Count == 0 || ListExamItems.Count == 0)
                MessageBox.Show("Not enough information!", "Error");
            else
                if (!importedDB)
                MessageBox.Show("Enter Database!", "Error");
            else
                if (!isConnectedToDB())
                MessageBox.Show("Please test connect to Sql Server", "Error");
            else
            {
                Hide();
                var Score = new Scoring(ListExamItems, ListSubmitions);
            }
        }

        private void checkConnectionButton_Click(object sender, EventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordTextBox.Text;
            string serverName = serverNameTextBox.Text;
            string initialCatalog = initialCatalogTextBox.Text;
            if (General.CheckConnection(serverName, username, password, initialCatalog))
                statusConnectCheckBox.Checked = true;
            else
                MessageBox.Show("Can not connect, check again.", "Error");
        }

        private void browseDatabases_Click(object sender, EventArgs e)
        {
            try
            {
                // Get directory where student's submittion was saved
                DBPath = FileUtils.GetFolderLocation();
                dbPathTextBox.Text = DBPath;
                // Get all submition files
                string[] DBScriptFiles = Directory.GetFiles(DBPath, "*.sql");
                if (DBScriptFiles.Count() == 0)
                    throw new Exception("No DB Script was found");
                else
                    // Get SQL Script files path successfully -> process to generate DB
                    LoadDBScript(DBScriptFiles);
            }
            catch (Exception ex)
            {
                dbPathTextBox.Text = "";
                MessageBox.Show("Imported DB Failed\n" + ex.Message, "Error");
            }
        }

        private void LoadDBScript(string[] dbScriptFiles)
        {
            int DBCreatedCount = 0;
            foreach (string scriptFile in dbScriptFiles)
            {
                DBCreatedCount++;
                // Run file to generate database
                try
                {
                    RunFile(scriptFile);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message + " at file " + Path.GetFileName(scriptFile), "Error");
                    throw e;
                }
            }
            // Get DB names and paths into constant
            try
            {
                // run sql 
                General.GetPathDB(DBCreatedCount);
                importedDB = true;
                browseDatabasesButton.Enabled = false;
                MessageBox.Show("Imported " + Constant.listDB.Length + " DB succesfully");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void RunFile(string url)
        {
            FileInfo file = new FileInfo(url);
            string script = file.OpenText().ReadToEnd();
            General.ExecuteSingleQuery(script);
        }
        private void statusConnectCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (isConnectedToDB())
            {
                usernameTextBox.Enabled = false;
                passwordTextBox.Enabled = false;
                serverNameTextBox.Enabled = false;
                initialCatalogTextBox.Enabled = false;
                statusConnectCheckBox.ForeColor = Color.Green;
                statusConnectCheckBox.Text = "Sql Connected";
                checkConnectionButton.Enabled = false;
                browseDatabasesButton.Enabled = true;
            }
        }
        private bool isConnectedToDB()
        {
            return statusConnectCheckBox.Checked;
        }

        private void ImportMaterial_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
